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
    public async Task<IEnumerable<BookCoverDto>> GetBookCoversProcessOneByOneAsync(Guid bookId,CancellationToken cancellationToken)
    {
        var httpclient = _httpClientFactory.CreateClient();
        var bookCovers = new List<BookCoverDto>();


        var bookCoverUrls = new[]
        {
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover1",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover2",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover3",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover4",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover5"
        };

        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            using (var linkedCancellationTokenSource= CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token,cancellationToken))
            {
                foreach (var bookCoverUrl in bookCoverUrls)
                {
                    var response = await httpclient.GetAsync(bookCoverUrl, linkedCancellationTokenSource.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        var bookCover = JsonSerializer.Deserialize<BookCoverDto>(await response.Content.ReadAsStringAsync(linkedCancellationTokenSource.Token),
                            new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true
                            });

                        if (bookCover != null)
                        {
                            bookCovers.Add(bookCover);
                        }
                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                    }
                }
            }
            
        }
        

        return bookCovers;
    }
    public async Task<IEnumerable<BookCoverDto>> GetBookCoversProcessAfterWaitForAllAsync(Guid bookId)
    {
        var httpclient = _httpClientFactory.CreateClient();
        var bookCovers = new List<BookCoverDto>();
        var bookCoverUrls = new[]
        {
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover1",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover2",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover3",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover4",
            $"https://localhost:44365/api/bookCovers/{bookId}-dummycover5"
        };

        var bookCoversTasks = bookCoverUrls.Select(bookCoverUrl => httpclient.GetAsync(bookCoverUrl)).ToList();

        var bookCoverTaskResults = await Task.WhenAll(bookCoversTasks);
        foreach (var bookCoverTaskResult in bookCoverTaskResults.Reverse())
        {
            var bookCover = JsonSerializer.Deserialize<BookCoverDto>(await bookCoverTaskResult.Content.ReadAsStringAsync(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

            if (bookCover != null)
            {
                bookCovers.Add(bookCover);
            }
        }
        return bookCovers;
    }
}