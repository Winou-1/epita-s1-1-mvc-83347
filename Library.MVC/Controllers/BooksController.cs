using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Domain;
using Library.MVC.Data;
using Library.MVC.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? searchTerm, string? category, string? availability)
        {
            IQueryable<Book> q = _context.Books;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));

            if (!string.IsNullOrWhiteSpace(category))
                q = q.Where(b => b.Category == category);

            if (availability == "available")
                q = q.Where(b => b.IsAvailable);
            else if (availability == "onloan")
                q = q.Where(b => !b.IsAvailable);

            var cats = await _context.Books
                .Select(b => b.Category).Distinct().OrderBy(c => c).ToListAsync();

            return View(new BookFilterViewModel
            {
                Books = await q.OrderBy(b => b.Title).ToListAsync(),
                SearchTerm = searchTerm,
                Category = category,
                Availability = availability,
                Categories = new SelectList(cats)
            });
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            if (book.Loans.Any())
            {
                TempData["Error"] = "Impossible de supprimer ce livre car il a des prêts associés.";
                return RedirectToAction(nameof(Index));
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
