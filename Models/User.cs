using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }
        [Required]
        public string UserName { get; set; }

        public int Role { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserRoles UserRole { get; set; }
    }
    public class CreateUser
    {
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }
        [Required]
        public int Role { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}