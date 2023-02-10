using Books.API.DbContexts;
using Books.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.API.Services;

public class BooksRepository : IBooksRepository
{
    private readonly BooksContext _context;

    public BooksRepository(BooksContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<Book> GetBooks()
    {
        return _context.Books!.ToList();
    }

    public Book? GetBook(Guid id)
    {
        return _context.Books!.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        return await _context.Books!.Include(x=>x.Author).ToListAsync();
    }

    public async Task<Book?> GetBookAsync(Guid id)
    {
        return await _context.Books!.Include(x=>x.Authorf).FirstOrDefaultAsync(x=>x.Id==id);
    }
}