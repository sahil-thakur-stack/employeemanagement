using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using Dapper;
using EmployeeManagement.Services;
using BCrypt.Net; // Import BCrypt.Net for password hashing

namespace EmployeeManagement.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserConnection _userConnection;
        private readonly EmployeeConnection _employeeConnection;

        public UsersController(UserConnection userConnection, EmployeeConnection employeeConnection, ILogger<UsersController> logger, IConfiguration configuration)
        {
            _userConnection = userConnection;
            _employeeConnection = employeeConnection;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Displays a list of all users.
        /// </summary>
        /// <returns>A view displaying the list of users.</returns>
        [JwtAuthentication]
        public async Task<IActionResult> Index()
        {
            using var connection = _userConnection.CreateConnection();
            string query = "SELECT * FROM tblUsers";
            var usersDapper = await connection.QueryAsync<User>(query);
            return View(usersDapper);
        }

        /// <summary>
        /// Displays the Privacy page.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>The Login view.</returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Processes the login form submission and authenticates the user.
        /// </summary>
        /// <param name="model">The login view model containing username and password.</param>
        /// <returns>Redirects to the Employees Index page on successful login; otherwise, redisplays the login form with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string query = "SELECT * FROM tblUsers WHERE UserName = @UserName";
                    using var connection = _userConnection.CreateConnection();
                    var user = await connection.QueryFirstOrDefaultAsync<User>(
                        query,
                        new { UserName = model.UserName }
                    );

                    if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        // Generate JWT Token
                        var jwtToken = Authentication.GenerateJWTAuthentication(user.UserName, "Admin", _configuration);

                        // Optionally, validate the token (this is more for debugging)
                        var validUserName = Authentication.ValidateToken(jwtToken, _configuration);

                        if (string.IsNullOrEmpty(validUserName))
                        {
                            ModelState.AddModelError("", "Unauthorized login attempt.");
                            return View();
                        }

                        // Set token in a secure HTTP-only cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // Uncomment this if running over HTTPS
                            SameSite = SameSiteMode.Strict
                        };
                        Response.Cookies.Append("jwt", jwtToken, cookieOptions);

                        // Store additional user information in the session if needed
                        //HttpContext.Session.SetString("UserID", user.ID.ToString()); // Assuming user.Id exists
                        //HttpContext.Session.SetString("UserName", user.UserName);

                        // Redirect to the appropriate action
                        return RedirectToAction("Index", "Employees");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during login.");
                    return StatusCode(500, "Internal server error");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Displays the user creation form.
        /// </summary>
        /// <returns>The Create view.</returns>
        public async Task<IActionResult> Create()
        {
            // Get the list of roles
            using var connection = _userConnection.CreateConnection();
            string query = "SELECT * FROM tblUsersRoles";
            var roles = await connection.QueryAsync<UserRoles>(query);

            // Pass the roles to the view
            ViewBag.Roles = roles;

            return View();
        }
        /// <summary>
        /// Processes the user creation form submission and adds a new user to the database.
        /// </summary>
        /// <param name="model">The user creation view model containing user details.</param>
        /// <returns>Redirects to the Index page on successful creation; otherwise, redisplays the creation form with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CreateUser user)
        {
            if (ModelState.IsValid)
            {
                // Save user logic here
                using var connection = _userConnection.CreateConnection();
                string insertQuery = @"
              INSERT INTO tblUsers (FirstName, LastName, UserName, Role, Password)
              VALUES (@FirstName, @LastName, @UserName, @Role, @Password)";

                var parameters = new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Role = user.Role,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password)   // Ideally, hash the password before saving
                };

                await connection.ExecuteAsync(insertQuery, parameters);

                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate the roles and return the view
            using var connectionForRoles = _userConnection.CreateConnection();
            string query = "SELECT * FROM tblUserRoles";  // Assuming a tblUserRoles table exists
            ViewBag.Roles = await connectionForRoles.QueryAsync<UserRoles>(query);

            return View(user);
        }

        [JwtAuthentication]

        public async Task<IActionResult> Edit(int id)
        {
            string query = "SELECT * FROM tblUsers WHERE Id = @Id";
            using var connection = _userConnection.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(
            query,
                new { Id = id }
            );
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel user)
        {
            if (user.ID == 0)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                string newQuery = "UPDATE tblUsers SET FirstName = @FirstName, LastName = @LastName, UserName = @UserName, Role = @Role WHERE ID = @ID";
                using var connection = _userConnection.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(newQuery, new
                {
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.Role,
                    user.ID
                });

                if (affectedRows > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update the user.");
                }
            }

            return View(user);
        }
        [JwtAuthentication]

        public async Task<IActionResult> Delete(int id)
        {
            string query = "SELECT * FROM tblUsers WHERE ID = @Id";
            using var connection = _userConnection.CreateConnection();
            var User = await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string query = "DELETE FROM tblUsers WHERE ID = @Id";
            using var connection = _userConnection.CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
            return RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// Handle Unauthorized Request
        /// </summary>
        /// <returns></returns>

        public IActionResult Unauthorized()
        {
            return View();
        }
    }
}
