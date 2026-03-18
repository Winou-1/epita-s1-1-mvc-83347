using Library.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.MVC.Views;

public class BookFilterViewModel
{
    public IEnumerable<Book> Books { get; set; } = [];
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? Availability { get; set; }
    public SelectList? Categories { get; set; }
}