using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        public float DateOfJoining { get; set; } = -1; // Sentinel value for unset date

        [Required]
        [PastDate]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int? Salary { get; set; }

        // Conversion properties
        [NotMapped]
        [Required(ErrorMessage = "Date of Joining is required.")]
        public DateTime? DateOfJoiningDateTime
        {
            get => DateOfJoining > 0 ? DateTimeConverter.UnixTimestampToDateTime(DateOfJoining) : (DateTime?)null;
            set => DateOfJoining = value.HasValue ? DateTimeConverter.DateTimeToUnixTimestamp(value.Value) : -1; // Sentinel value
        }

        [NotMapped]
        public string DateOfJoiningFormatted
        {
            get => DateOfJoiningDateTime.HasValue ? DateOfJoiningDateTime.Value.ToString("MM/dd/yyyy") : "N/A";
        }
    }
}
