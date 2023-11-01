namespace Api.Dto.Common;

public class ResponseDto
{
    public ResponseDto()
    {
    }

    public ResponseDto(string? message, bool success = false)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; } = new List<string>();
    public string? TraceId { get; set; }

    public static ResponseDto Successful()
    {
        return new ResponseDto() { Success = true };
    }

    public static ResponseDto Failure(string message, IEnumerable<string>? errors)
    {
        var result = new ResponseDto { Success = false, Message = message };
        if (errors != null)
            result.Errors = errors.ToList();
        return result;
    }

    public static ResponseDto<T> Failure<T>(string message)
    {
        return new ResponseDto<T> { Success = false, Message = message };
    }

    public static ResponseDto<T> Successful<T>(T result)
    {
        return new ResponseDto<T> { Result = result, Success = true };
    }
}

public class ResponseDto<T> : ResponseDto
{
    public ResponseDto()
    {
    }

    public ResponseDto(T data, string? message, bool success = false) : base(message, success)
    {
        Result = data;
    }

    public T? Result { get; set; }
}