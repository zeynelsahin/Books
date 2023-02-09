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
    public string Name { get; set; }

    [Required]
    [MaxLength(150)]
    public string LastName { get; set; }

    public Author(Guid type, string name, string lastName)
    {
        Type = type;
        Name = name;
        LastName = lastName;
    }
}