using Bogus;
using Library.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data;

public static class SeedData
{
    public static async Task InitialiseAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var manag = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManag = services.GetRequiredService<RoleManager<IdentityRole>>();
        await context.Database.MigrateAsync();

        if (!await roleManag.RoleExistsAsync("Admin"))
            await roleManag.CreateAsync(new IdentityRole("Admin"));

        const string adminEmail = "admin@bibliotheque.fr";
        if (await manag.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await manag.CreateAsync(admin, "Admin@1234");
            await manag.AddToRoleAsync(admin, "Admin");
        }

        if (await context.Books.AnyAsync()) return;

        var fake = new Faker("fr");
        var b1 = new Book { Title = "Les Misérables", Author = "Victor Hugo", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        var b2 = new Book { Title = "Le Petit Prince", Author = "Antoine de Saint-Exupéry", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        var b3 = new Book { Title = "Germinal", Author = "Émile Zola", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        var b4 = new Book { Title = "Madame Bovary", Author = "Gustave Flaubert", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        var b5 = new Book { Title = "L'Étranger", Author = "Albert Camus", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        var b6 = new Book { Title = "Voyage au bout de la nuit", Author = "Louis-Ferdinand Céline", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = false };
        var b7 = new Book { Title = "À la recherche du temps perdu", Author = "Marcel Proust", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = false };
        var b8 = new Book { Title = "Le Rouge et le Noir", Author = "Stendhal", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = false };
        var b9 = new Book { Title = "Notre-Dame de Paris", Author = "Victor Hugo", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = false };
        var b10 = new Book { Title = "Vingt mille lieues sous les mers", Author = "Jules Verne", Isbn = fake.Commerce.Ean13(), Category = "Aventure", IsAvailable = false };
        var b11 = new Book { Title = "Le Tour du monde en 80 jours", Author = "Jules Verne", Isbn = fake.Commerce.Ean13(), Category = "Aventure", IsAvailable = false };
        var b12 = new Book { Title = "Les Trois Mousquetaires", Author = "Alexandre Dumas", Isbn = fake.Commerce.Ean13(), Category = "Aventure", IsAvailable = false };
        var b13 = new Book { Title = "Le Comte de Monte-Cristo", Author = "Alexandre Dumas", Isbn = fake.Commerce.Ean13(), Category = "Aventure", IsAvailable = false };
        var b14 = new Book { Title = "Candide", Author = "Voltaire", Isbn = fake.Commerce.Ean13(), Category = "Philosophie", IsAvailable = false };
        var b15 = new Book { Title = "Les Fleurs du mal", Author = "Charles Baudelaire", Isbn = fake.Commerce.Ean13(), Category = "Poésie", IsAvailable = false };
        var b16 = new Book { Title = "Astérix le Gaulois", Author = "René Goscinny", Isbn = fake.Commerce.Ean13(), Category = "BD", IsAvailable = true };
        var b17 = new Book { Title = "Tintin au Tibet", Author = "Hergé", Isbn = fake.Commerce.Ean13(), Category = "BD", IsAvailable = true };
        var b18 = new Book { Title = "La Cuisine de Bocuse", Author = "Paul Bocuse", Isbn = fake.Commerce.Ean13(), Category = "Cuisine", IsAvailable = true };
        var b19 = new Book { Title = "Histoire de France", Author = "Jules Michelet", Isbn = fake.Commerce.Ean13(), Category = "Histoire", IsAvailable = true };
        var b20 = new Book { Title = "Extension du domaine de la lutte", Author = "Michel Houellebecq", Isbn = fake.Commerce.Ean13(), Category = "Fiction", IsAvailable = true };
        context.Books.AddRange(b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20);
        var m1 = new Member { FullName = "Jean-Pierre Dupont", Email = "jean.dupont@orange.fr", Phone = fake.Phone.PhoneNumber("06########") };
        var m2 = new Member { FullName = "Marie-Claire Lefevre", Email = "mariiiiiiie.lefevre@free.fr", Phone = fake.Phone.PhoneNumber("07########") };
        var m3 = new Member { FullName = "Jacques Moreau", Email = "canrd.moreau@laposte.net", Phone = fake.Phone.PhoneNumber("06########") };
        var m4 = new Member { FullName = "Isabelle Petit", Email = "guiness.petit@gmail.com", Phone = fake.Phone.PhoneNumber("07########") };
        var m5 = new Member { FullName = "François Renard", Email = "murph.renard@sfr.fr", Phone = fake.Phone.PhoneNumber("06########") };
        var m6 = new Member { FullName = fake.Name.FullName(), Email = fake.Internet.Email(), Phone = fake.Phone.PhoneNumber("07########") };
        var m7 = new Member { FullName = fake.Name.FullName(), Email = fake.Internet.Email(), Phone = fake.Phone.PhoneNumber("06########") };
        var m8 = new Member { FullName = fake.Name.FullName(), Email = fake.Internet.Email(), Phone = fake.Phone.PhoneNumber("07########") };
        var m9 = new Member { FullName = fake.Name.FullName(), Email = fake.Internet.Email(), Phone = fake.Phone.PhoneNumber("06########") };
        var m10 = new Member { FullName = fake.Name.FullName(), Email = fake.Internet.Email(), Phone = fake.Phone.PhoneNumber("07########") };
        context.Members.AddRange(m1, m2, m3, m4, m5, m6, m7, m8, m9, m10);
        await context.SaveChangesAsync();
        var today = DateTime.Today;
        context.Loans.Add(new Loan { Book = b1, Member = m1, LoanDate = today.AddDays(-32), DueDate = today.AddDays(-26), ReturnedDate = today.AddDays(-30) });
        context.Loans.Add(new Loan { Book = b2, Member = m3, LoanDate = today.AddDays(-7), DueDate = today.AddDays(-21), ReturnedDate = today.AddDays(-22) });
        context.Loans.Add(new Loan { Book = b3, Member = m5, LoanDate = today.AddDays(-120), DueDate = today.AddDays(-36), ReturnedDate = today.AddDays(-38) });
        context.Loans.Add(new Loan { Book = b4, Member = m7, LoanDate = today.AddDays(-53), DueDate = today.AddDays(-46), ReturnedDate = today.AddDays(-47) });
        context.Loans.Add(new Loan { Book = b5, Member = m9, LoanDate = today.AddDays(-45), DueDate = today.AddDays(-31), ReturnedDate = today.AddDays(-32) });
        context.Loans.Add(new Loan { Book = b6, Member = m2, LoanDate = today.AddDays(-3), DueDate = today.AddDays(11) });
        context.Loans.Add(new Loan { Book = b7, Member = m4, LoanDate = today.AddDays(-5), DueDate = today.AddDays(9) });
        context.Loans.Add(new Loan { Book = b8, Member = m6, LoanDate = today.AddDays(-1), DueDate = today.AddDays(13) });
        context.Loans.Add(new Loan { Book = b9, Member = m8, LoanDate = today.AddDays(-7), DueDate = today.AddDays(7) });
        context.Loans.Add(new Loan { Book = b10, Member = m10, LoanDate = today.AddDays(-9), DueDate = today.AddDays(12) });
        context.Loans.Add(new Loan { Book = b11, Member = m1, LoanDate = today.AddDays(-30), DueDate = today.AddDays(-16) });
        context.Loans.Add(new Loan { Book = b12, Member = m3, LoanDate = today.AddDays(-27), DueDate = today.AddDays(-11) });
        context.Loans.Add(new Loan { Book = b13, Member = m5, LoanDate = today.AddDays(-48), DueDate = today.AddDays(-14) });
        context.Loans.Add(new Loan { Book = b14, Member = m7, LoanDate = today.AddDays(-20), DueDate = today.AddDays(-6) });
        context.Loans.Add(new Loan { Book = b15, Member = m9, LoanDate = today.AddDays(-22), DueDate = today.AddDays(-8) });
        await context.SaveChangesAsync();
    }
}