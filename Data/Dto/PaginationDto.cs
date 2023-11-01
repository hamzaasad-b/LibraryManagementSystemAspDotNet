namespace Data.Dto;

public class PaginationDto<T>
{
    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalRecords { get; set; }

    public PaginationDto(IEnumerable<T> result, int page, int size, long total)
    {
        Data = result;
        PageNumber = page;
        PageSize = size;
        TotalRecords = total;
    }
}