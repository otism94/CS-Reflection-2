using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reflection.Models;

namespace Reflection.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Context context)
        {
            context.Database.EnsureCreated();

            // Check if any company data exists.
            if (context.Companies.Any())
            {
                return; // DB is seeded
            }

            var employees = new Employee[]
            {
                new Employee()
                {
                    FirstName = "Joseph",
                    LastName = "Joestar",
                    Email = "joseph@speedwagon.org",
                    Phone = "03957937583",
                    CompanyId = 1
                },
                new Employee()
                {
                    FirstName = "Rufus",
                    LastName = "Shinra",
                    Email = "boss@shinra.com",
                    Phone = "09472019746",
                    CompanyId = 2
                },
                new Employee()
                {
                    FirstName = "Calyssia",
                    LastName = "Rai-Paulsen",
                    Email = "scrapdept@lynxcorp.com",
                    Phone = "08477940166",
                    CompanyId = 3
                }
            };

            foreach (Employee employee in employees)
            {
                context.Employees.Add(employee);
            }

            context.SaveChanges();

            var companies = new Company[]
            {
                new Company()
                {
                    Name = "Speedwagon Foundation",
                    Email = "contact@speedwagon.org",
                    Logo = "speedwagon-foundation.jpg",
                    Website = "https://speedwagonfoundation.org/"
                },
                new Company()
                {
                    Name = "ShinRa Electric Power Company",
                    Email = "hello@shinra.com",
                    Logo = "shinra-co.jpg",
                    Website = "https://shinra.com/"
                },
                new Company()
                {
                    Name = "LYNX Corp",
                    Email = "contact@lynxcorp.com",
                    Logo = "lynxcorp.jpg",
                    Website = "https://lynx.corp.com/"
                }
            };

            foreach (Company company in companies)
            {
                context.Companies.Add(company);
            }

            context.SaveChanges();
        }
    }
}
