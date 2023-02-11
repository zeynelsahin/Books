using System.Text.Json;
using Books.API.DbContexts;
using Books.API.Entities;
using Books.API.Models.External;
using Microsoft.EntityFrameworkCore;

namespace Books.API.Services;

public class BooksRepository : IBooksRepository
{
    private readonly BooksContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksRepository(BooksContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public IEnumerable<Book> GetBooks()
    {
        return _context.Books!.ToList();
    }

    public Book? GetBook(Guid id)
    {
        return _context.Books!.FirstOrDefault(p => p.Id == id);
    }
    public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
    {
        return await _context.Books!.Where(b => bookIds.Contains(b.Id)).Include(b => b.Author).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        return await _context.Books!.Include(x => x.Author).ToListAsync();
    }
    public IAsyncEnumerable<Book> GetBooksAsAsyncEnumerable()
    {
        return _context.Books!.AsAsyncEnumerable<Book>();
    }

    public async Task<Book?> GetBookAsync(Guid id)
    {
        return await _context.Books!.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
    }

    public void AddBook(Book bookToAdd)
    {
        if (bookToAdd == null)
        {
            throw new ArgumentNullException(nameof(bookToAdd));
        }

        _context.Books!.Add(bookToAdd);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<BookCoverDto?> GetBookCoverAsync(string id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync($"https://localhost:44365/api/bookCovers/{id}");

        return response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<BookCoverDto>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                })
            : null;
    }
}