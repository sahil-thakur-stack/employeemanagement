using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class EditUserViewModel
    {
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }
        [Required]
        public string UserName { get; set; }

        public int Role { get; set; }
    }
}
