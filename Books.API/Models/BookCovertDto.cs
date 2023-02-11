namespace Books.API.Models;

public class BookCoverDto
{
    public BookCoverDto(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
    // public byte[] Content { get; set; }
    // public BookCoverDto(string id, byte[] content)
    // {
    //     Id = id;
    //     Content = content;
    // }
}