using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
  public class UserRoles
  {
    public int ID { get; set; }
    [Required]
    public string UserRole { get; set; }
  }
}
