using AutoMapper;
using Books.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Books.API.Filters;

public class BookWithCoversResultFilter : IAsyncResultFilter
{

    private readonly IMapper _mapper;
    public BookWithCoversResultFilter(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var resultFromAction = context.Result as ObjectResult;

        if (resultFromAction?.Value == null || resultFromAction.StatusCode is < 200 or >= 300)
        {
            await next();
        }

        var (book, bookCovers) = ((Entities.Book book, IEnumerable<Models.External.BookCoverDto> bookCovers))resultFromAction!.Value!;

        var mappedBook = _mapper.Map<BookWithCoversDto>(book);
        resultFromAction.Value = _mapper.Map(bookCovers, mappedBook);

        await next();
    }
}