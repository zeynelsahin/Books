﻿using AutoMapper;
using Books.API.Entities;
using Books.API.Filters;
using Books.API.Helpers;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[Route("api/bookcollections")]
[ApiController]
[TypeFilter(typeof(BooksResultFilter))]
public class BookCollectionController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    public BookCollectionController(IBooksRepository booksRepository, IMapper mapper)
    {
        _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookCollection([FromBody]IEnumerable<BookForCreationDto> bookCollection)
    {
        var bookEntities = _mapper.Map<IEnumerable<Book>>(bookCollection);
        foreach (var bookEntity in bookEntities) _booksRepository.AddBook(bookEntity);
        await _booksRepository.SaveChangesAsync();
        var booksToReturn = await _booksRepository.GetBooksAsync(bookEntities.Select(b => b.Id).ToList());

        var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));
        return CreatedAtRoute("GetBookCollection", new { bookIds }, booksToReturn);
    } 
    
    [HttpGet("{bookIds}", Name = "GetBookCollection")]
    public async Task<IActionResult> GetBookCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds)
    {
        bookIds = bookIds.ToArray();
        var bookEntities = await _booksRepository.GetBooksAsync(bookIds);

        if (bookEntities.Count() != bookIds.Count()) return NotFound();

        return Ok(bookEntities);
    }
}