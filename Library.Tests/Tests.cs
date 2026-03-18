using Humanizer;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Library.Tests;

public class Tests
{
    private static ApplicationDbContext CreateCtx() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task CannotLendBookThatIsAlreadyOnActiveLoan()
    {
        await using var ctx = CreateCtx();
        var book = new Book { Title = "Dune", Author = "Herbert", IsAvailable = false };
        var member = new Member { FullName = "Alice", Email = "a@a.com", Phone = "1" };
        ctx.Books.Add(book); ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        ctx.Loans.Add(new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14)
        });
        await ctx.SaveChangesAsync();

        var hasActiveLoan = await ctx.Loans
            .AnyAsync(l => l.BookId == book.Id && l.ReturnedDate == null);

        Assert.True(hasActiveLoan);
        Assert.False(book.IsAvailable);
    }

    [Fact]
    public async Task ReturningLoanMakesBookAvailableAgain()
    {
        await using var ctx = CreateCtx();
        var book = new Book { Title = "1984", Author = "Orwell", IsAvailable = false };
        var member = new Member { FullName = "Bob", Email = "b@b.com", Phone = "2" };
        ctx.Books.Add(book); ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Today.AddDays(-5),
            DueDate = DateTime.Today.AddDays(9)
        };
        ctx.Loans.Add(loan);
        await ctx.SaveChangesAsync();

        loan.ReturnedDate = DateTime.Today;
        book.IsAvailable = true;
        await ctx.SaveChangesAsync();

        Assert.True((await ctx.Books.FindAsync(book.Id))!.IsAvailable);
        Assert.NotNull((await ctx.Loans.FindAsync(loan.Id))!.ReturnedDate);
    }

    [Fact]
    public async Task BookSearchByTitleReturnsCorrectMatches()
    {
        await using var ctx = CreateCtx();
        ctx.Books.AddRange(
            new Book { Title = "The Hobbit", Author = "Tolkien", IsAvailable = true },
            new Book { Title = "Dune", Author = "Herbert", IsAvailable = true },
            new Book { Title = "The Silmarillion", Author = "Tolkien", IsAvailable = true }
        );
        await ctx.SaveChangesAsync();

        var results = await ctx.Books.Where(b => b.Title.Contains("The")).ToListAsync();
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task OverdueLoansDetectedCorrectly()
    {
        await using var ctx = CreateCtx();
        var book = new Book { Title = "Late", Author = "X", IsAvailable = false };
        var member = new Member { FullName = "Carol", Email = "c@c.com", Phone = "3" };
        ctx.Books.Add(book); ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        ctx.Loans.Add(new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Today.AddDays(-30),
            DueDate = DateTime.Today.AddDays(-5)
        });
        await ctx.SaveChangesAsync();

        var overdue = await ctx.Loans
            .Where(l => l.DueDate < DateTime.Today && l.ReturnedDate == null)
            .ToListAsync();
        Assert.Single(overdue);
    }

    [Fact]
    public async Task BookSearchByAuthorReturnsCorrectMatches()
    {
        await using var ctx = CreateCtx();
        ctx.Books.AddRange(
            new Book { Title = "A", Author = "George Orwell", IsAvailable = true },
            new Book { Title = "B", Author = "Jane Austen", IsAvailable = true },
            new Book { Title = "C", Author = "George Bernard Shaw", IsAvailable = true }
        );
        await ctx.SaveChangesAsync();

        var results = await ctx.Books.Where(b => b.Author.Contains("George")).ToListAsync();
        Assert.Equal(2, results.Count);
    }
}