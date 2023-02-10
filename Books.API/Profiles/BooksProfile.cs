﻿using AutoMapper;
using Books.API.Controllers;
using Books.API.Entities;
using Books.API.Models;

namespace Books.API.Profiles;

public class BooksProfile : Profile
{
    public BooksProfile()
    {
        
        CreateMap<Book, BookDto>().ForMember(destination => destination.AuthorName, option => option.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}")).ConstructUsing(src => new BookDto(src.Id, string.Empty, src.Title, src.Description));
        
        CreateMap<BookForCreationDto, Book>()
            .ConstructUsing(src => new Book(Guid.NewGuid(),
                src.AuthorId,
                src.Title,
                src.Description));
    }
}