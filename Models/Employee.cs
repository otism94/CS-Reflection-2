using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Reflection.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        [MinLength(11)]
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
