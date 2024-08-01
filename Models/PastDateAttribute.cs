using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class PastDateAttribute : ValidationAttribute
    {
        public PastDateAttribute()
        {
            ErrorMessage = "The date cannot be in the future.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
