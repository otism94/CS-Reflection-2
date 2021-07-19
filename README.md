# CS-Reflection-2
My C# Reflection 2 for the Netmatters SCS training course.  
  
Created in Visual Studio 2019 with ASP.NET Core and Entity Framework Core. Tested working in Chrome only.

## Set-Up
1. Install all required NuGet packages.
2. Adjust the connection string in `appsettings.json` if necessary. By default, the server is `(localdb)\\mssqllocaldb` and the database name is `OtisMoormanReflection`.
3. Open Package Manager Console and enter `Update-Database`.
4. Everything should be ready to go.

## Packages
- Microsoft.AspNetCore.Disagnostics.EntityFrameworkCore v5.0.8
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation v5.0.8
- Microsoft.EntityFrameworkCore v5.0.8
- Microsoft.EntityFrameworkCore.Design v5.0.8
- Microsoft.EntityFrameworkCore.Relational v5.0.8
- Microsoft.EntityFrameworkCore.SqlServer v5.0.8
- Microsoft.EntityFrameworkCore.Tools v5.0.8
- Microsoft.Extensions.Options.DataAnnotations v5.0.0 (this may be unused)
- Microsoft.VisualStudio.Web.CodeGeneration.Design v5.0.2
- System.Drawing.Common v5.0.2

## Known Issues
- File upload can crash the site when running in debug mode depending on the solution's port configuration. Running without debugging is recommended.
