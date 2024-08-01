using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using EmployeeManagement.Models;

using EmployeeManagement.Controllers;


[JwtAuthentication]
public class EmployeesController : Controller
{
    private readonly ILogger<EmployeesController> _logger;
    private readonly EmployeeDbContext _context;
    private readonly IDbConnection _dapperConnection;
    private readonly IConfiguration _configuration;
    private readonly UserConnection _userConnection;
    private readonly EmployeeConnection _employeeConnection;
    public EmployeesController(UserConnection userConnection, EmployeeConnection employeeConnection, ILogger<EmployeesController> logger)
    {
        _userConnection = userConnection;
        _employeeConnection = employeeConnection;
        _logger = logger;
    }


    /// <summary>
    /// Retrieves and displays a list of all employees.
    /// </summary>
    /// <returns>A view displaying the list of employees.</returns>
    public async Task<IActionResult> Index()
    {
        string query = "SELECT * FROM tblEmployees";
        using var connection = _employeeConnection.CreateConnection();
        var employees = await connection.QueryAsync<Employee>(query);
        return View(employees);
    }

    /// <summary>
    /// Retrieves and displays details of a specific employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <returns>A view displaying the details of the employee if found; otherwise, a 404 Not Found result.</returns>
    public async Task<IActionResult> Details(int id)
    {
        string query = "SELECT * FROM tblEmployees WHERE ID = @Id";
        using var connection = _employeeConnection.CreateConnection();
        var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    /// <summary>
    /// Displays the form to create a new employee.
    /// </summary>
    /// <returns>The view to create a new employee.</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Processes the form submission to create a new employee.
    /// </summary>
    /// <param name="employee">The employee model to create.</param>
    /// <returns>Redirects to the Index action on success; otherwise, redisplays the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,EmployeeCode,DateOfJoiningDateTime,DateOfBirth,Salary")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            if (await EmployeeCodeExists(employee.EmployeeCode))
            {
                ModelState.AddModelError("EmployeeCode", "Employee code already exists.");
                return View(employee);
            }
            if (!employee.DateOfJoiningDateTime.HasValue)
            {
                ModelState.AddModelError("DateOfJoiningDateTime", "Date of Joining is required.");
                return View(employee);
            }
            string query = "INSERT INTO tblEmployees (FirstName, LastName, EmployeeCode, DateOfJoining, DateOfBirth, Salary) VALUES (@FirstName, @LastName, @EmployeeCode, @DateOfJoining, @DateOfBirth, @Salary)";
            using var connection = _employeeConnection.CreateConnection();
            await connection.ExecuteAsync(query, employee);
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    /// <summary>
    /// Retrieves and displays the form to edit an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to edit.</param>
    /// <returns>The view to edit the employee if found; otherwise, a 404 Not Found result.</returns>
    public async Task<IActionResult> Edit(int id)
    {
        string query = "SELECT * FROM tblEmployees WHERE ID = @Id";
        using var connection = _employeeConnection.CreateConnection();
        var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    /// <summary>
    /// Processes the form submission to update an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="employee">The updated employee model.</param>
    /// <returns>Redirects to the Index action on success; otherwise, redisplays the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,EmployeeCode,DateOfJoiningDateTime,DateOfBirth,Salary")] Employee employee)
    {
        if (id != employee.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (!employee.DateOfJoiningDateTime.HasValue)
            {
                ModelState.AddModelError("DateOfJoiningDateTime", "Date of Joining is required.");
                return View(employee);
            }
            string query = "UPDATE tblEmployees SET FirstName = @FirstName, LastName = @LastName, EmployeeCode = @EmployeeCode, DateOfJoining = @DateOfJoining, DateOfBirth = @DateOfBirth, Salary = @Salary WHERE ID = @ID";
            using var connection = _employeeConnection.CreateConnection();
            await connection.ExecuteAsync(query, employee);
            return RedirectToAction(nameof(Index));
        }

        return View(employee);
    }

    /// <summary>
    /// Retrieves and displays the details of an employee to be deleted.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>The view displaying the details of the employee if found; otherwise, a 404 Not Found result.</returns>
    public async Task<IActionResult> Delete(int id)
    {
        string query = "SELECT * FROM tblEmployees WHERE ID = @Id";
        using var connection = _employeeConnection.CreateConnection();
        var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    /// <summary>
    /// Confirms and processes the deletion of an employee.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>Redirects to the Index action after deletion.</returns>
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        string query = "DELETE FROM tblEmployees WHERE ID = @Id";
        using var connection = _employeeConnection.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Checks if an employee code already exists in the database.
    /// </summary>
    /// <param name="employeeCode">The employee code to check.</param>
    /// <returns>True if the employee code exists; otherwise, false.</returns>
    [NonAction]
    private async Task<bool> EmployeeCodeExists(string employeeCode)
    {
        string query = "SELECT COUNT(*) FROM tblEmployees WHERE EmployeeCode = @EmployeeCode";
        using var connection = _employeeConnection.CreateConnection();
        int count = await connection.ExecuteScalarAsync<int>(query, new { EmployeeCode = employeeCode });
        return count > 0;
    }

}
