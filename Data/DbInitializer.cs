using Reflection.Models;
using System.Linq;

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

            var companies = new Company[]
            {
                new Company()
                {
                    Name = "Speedwagon Foundation",
                    Email = "contact@speedwagon.org",
                    LogoName = "SPW.png",
                    Website = "https://speedwagonfoundation.org/"
                },
                new Company()
                {
                    Name = "ShinRa Electric Power Company",
                    Email = "hello@shinra.com",
                    LogoName = "shinra.jpg",
                    Website = "https://shinra.com/"
                },
                new Company()
                {
                    Name = "LYNX Corp",
                    Email = "contact@lynxcorp.com",
                    LogoName = "lynx.jpg",
                    Website = "https://lynx.corp.com/"
                }
            };

            foreach (Company company in companies)
            {
                context.Companies.Add(company);
            }

            context.SaveChanges();

            var employees = new Employee[]
            {
                new Employee()
                {
                    FirstName = "Robert",
                    LastName = "Speedwagon",
                    Email = "reospeedwagon@speedwagon.org",
                    Phone = "00498477938",
                    CompanyId = 1
                },
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
                    FirstName = "Lucy",
                    LastName = "Steel",
                    Email = "tickettoride@speedwagon.org",
                    Phone = "08856977463",
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
                    FirstName = "Reeve",
                    LastName = "Tuesti",
                    Email = "notcaitsith@shinra.com",
                    Phone = "09019858836",
                    CompanyId = 2
                },
                new Employee()
                {
                    FirstName = "Zack",
                    LastName = "Fair",
                    Email = "soldier@shinra.com",
                    Phone = "09998476635",
                    CompanyId = 2
                },
                new Employee()
                {
                    FirstName = "Exeter",
                    LastName = "Schlessinger",
                    Email = "ceo@lynxcorp.com",
                    Phone = "089375674884",
                    CompanyId = 3
                },
                new Employee()
                {
                    FirstName = "Calyssia",
                    LastName = "Rai-Paulsen",
                    Email = "scrapdept@lynxcorp.com",
                    Phone = "08477940166",
                    CompanyId = 3
                },
                new Employee()
                {
                    FirstName = "Weaver",
                    LastName = "McWeaver",
                    Email = "weaver@lynxcorp.com",
                    Phone = "08664583940",
                    CompanyId = 3
                }
            };

            foreach (Employee employee in employees)
            {
                context.Employees.Add(employee);
            }

            context.SaveChanges();
        }
    }
}
