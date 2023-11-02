using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class User : IdentityUser<uint>
{
    public string FullName { get; set; }
    public IEnumerable<Book> Books { get; set; }
}