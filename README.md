ASP.NET Core MVC app to manage books, members and loans.

Stack
- ASP.NET Core MVC (.NET 10)
- EF Core + SQL Server
- ASP.NET Identity
- xUnit tests

Login Admin
- Email: admin@bibliotheque.fr
- Password: Admin@1234

## Run
```bash
dotnet ef database update --project Library.MVC
dotnet run --project Library.MVC
```