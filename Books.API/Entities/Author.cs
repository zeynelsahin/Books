using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.API.Entities;
[Table("Authors")]
public class Author
{
    [Key]
    public Guid Type { get; set; }

    [Required]
    [MaxLength(150)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(150)]
    public string LastName { get; set; }

    public Author(Guid type, string firstName, string lastName)
    {
        Type = type;
        FirstName = firstName;
        LastName = lastName;
    }
}