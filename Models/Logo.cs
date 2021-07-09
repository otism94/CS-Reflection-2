using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Models
{
    public class Logo
    {
        public string LogoId { get; set; }
        public string LogoName { get; set; }
        [NotMapped]
        [DisplayName("Logo")]
        public IFormFile LogoFile { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
