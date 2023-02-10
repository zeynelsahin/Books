using AutoMapper;
using Books.API.Filters;
using Books.API.Services;
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
    public async Task<IActionResult> GetBooks()
    {
        var bookEntities = await _booksRepository.GetBooksAsync();
        return Ok(bookEntities);
    }

    [HttpGet("books/{id}", Name = "GetBookAsync")]
    [TypeFilter(typeof(BookResultFilter))]
    public async Task<IActionResult> GetBooks(Guid id)
    {
        var bookEntity = await _booksRepository.GetBookAsync(id);
        if (bookEntity == null)
        {
            return NotFound();
        }
        return Ok(bookEntity);
    }

    [HttpPost("books")]
    [TypeFilter(typeof(BookResultFilter))]
    public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookForCreationDto)
    {
        var bookEntity = _mapper.Map<Entities.Book>(bookForCreationDto);
        _booksRepository.AddBook(bookEntity);
        await _booksRepository.SaveChangesAsync();
        await _booksRepository.GetBookAsync(bookEntity.Id);
        return CreatedAtRoute("GetBookAsync", new { id = bookEntity.Id }, bookEntity );
    }
}