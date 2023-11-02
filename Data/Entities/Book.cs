namespace Data.Entities;

public class Book
{
    public uint Id { set; get; }
    public uint? IssuedToUserId { set; get; }
    public string Iban { get; set; }
    public string Title { get; set; }
    public List<Book> Books { get; set; }
    public User User { get; set; }
}