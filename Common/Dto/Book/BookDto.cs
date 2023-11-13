namespace Common.Dto.Book;

public class BookDto : IDto
{
    public uint? Id { get; set; }
    public uint? IssuedToUserId { get; set; }
    public string Iban { get; set; }
    public string Title { get; set; }
}