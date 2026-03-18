using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Library.Domain;

namespace Library.MVC.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book).WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Member).WithMany(m => m.Loans)
                .HasForeignKey(l => l.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
