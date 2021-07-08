using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reflection.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
