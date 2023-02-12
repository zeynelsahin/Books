using AutoMapper;
using Books.API.Entities;
using Books.API.Filters;
using Books.API.Models;
using Books.API.Services;
using Books.Legacy;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[Route("api")]
[ApiController]
public class BooksController : ControllerBase
{

    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;

    public BooksController(IBooksRepository booksRepository, IMapper mapper)
    {
        _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        _mapper = mapper;
    }

    [HttpGet("books")]
    [TypeFilter(typeof(BooksResultFilter))]
    public  IActionResult GetBooks_BadCode()
    {
        var bookEntities =  _booksRepository.GetBooksAsync().Result;
        return Ok(bookEntities);
    }
    [HttpGet("booksstream")]
    public async IAsyncEnumerable<BookDto> StreamBooks()
    {
        await foreach (var book in _booksRepository.GetBooksAsAsyncEnumerable())
        {
            await Task.Delay(500);
            yield return _mapper.Map<BookDto>(book);
        }
    }

    [HttpGet("books/{id}", Name = "GetBookAsync")]
    [TypeFilter(typeof(BookWithCoversResultFilter))]
    // [TypeFilter(typeof(BookResultFilter))]
    public async Task<IActionResult> GetBooks(Guid id, CancellationToken cancellationToken)
    {
        var bookEntity = await _booksRepository.GetBookAsync(id);
        if (bookEntity == null)
        {
            return NotFound();
        }


        // var amountOfPages = await GetBookPages_BadCode(id);
        // var bookCover = await _booksRepository.GetBookCoverAsync("dummycover");

        var bookCovers= await _booksRepository.GetBookCoversProcessOneByOneAsync(id,cancellationToken) ;
        // var bookCovers = await _booksRepository.GetBookCoversProcessAfterWaitForAllAsync(id);

        //if (cancellationToken.IsCancellationRequested) { } 
        return Ok(( bookEntity, bookCovers));
    }

    private Task<int> GetBookPages_BadCode(Guid id)
    {
        return Task.Run(() =>
        {
            var pageCalculator = new ComplicatedPageCalculator();
            return pageCalculator.CalculateBookPages(id);
        });
    }
    [HttpPost("books")]
    [TypeFilter(typeof(BookResultFilter))]
    public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookForCreationDto)
    {
        var bookEntity = _mapper.Map<Entities.Book>(bookForCreationDto);
        _booksRepository.AddBook(bookEntity);
        await _booksRepository.SaveChangesAsync();
        await _booksRepository.GetBookAsync(bookEntity.Id);
        return CreatedAtRoute("GetBookAsync", new { id = bookEntity.Id }, bookEntity);
    }
}