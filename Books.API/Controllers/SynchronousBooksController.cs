using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[Route("api")]
[ApiController]
public class SynchronousBooksController: ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    public SynchronousBooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    [HttpGet("books/sync")]
    public  IActionResult GetBooks()
    {
        var bookEntities =  _booksRepository.GetBooks();
        return Ok(bookEntities); 
    }
}