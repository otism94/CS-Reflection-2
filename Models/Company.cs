using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string Website { get; set; }
        [DisplayName("Logo")]
        public string LogoName { get; set; }
        [NotMapped]
        [DisplayName("Logo")]
        public IFormFile LogoFile { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
