using Microsoft.AspNetCore.Http;
using Reflection.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reflection.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(100, ErrorMessage = "Company name must be 100 characters or less.")]
        [MinLength(2, ErrorMessage = "Company name must be at least 2 characters long.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email address must be 100 characters or less.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL with http(s):// prefix.")]
        [StringLength(100, ErrorMessage = "Website must be 100 characters or less.")]
        public string Website { get; set; }

        [DisplayName("Logo")]
        public string LogoName { get; set; }

        [NotMapped]
        [DisplayName("Logo")]
        [MaxFileSize(5000000)]
        [MinDimensions(100)]
        [ImageExtensions]
        public IFormFile LogoFile { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
