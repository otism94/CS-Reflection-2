using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Reflection.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, ErrorMessage = "First name must be 50 characters or less.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, ErrorMessage = "Last name must be 50 characters or less.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email address must be 100 characters or less.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30, ErrorMessage = "Phone number must be 30 characters or less.")]
        [MinLength(11, ErrorMessage = "Phone number must be at least 11 characters long.")]
        public string Phone { get; set; }

        [DisplayName("Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }

        [DisplayName("Company")]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
