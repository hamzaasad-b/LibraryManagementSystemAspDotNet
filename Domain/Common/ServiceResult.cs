namespace Domain.Common;

/// <summary>
/// Represents the result of a service method call
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public virtual T? Data { get; set; }

    public List<string> Errors { get; set; } = new List<string>();

    public List<string>? ErrorCodes { get; set; }

    public static ServiceResult<T> SuccessfulFactory(T data)
    {
        return new ServiceResult<T>()
        {
            Data = data,
            Success = true
        };
    }

    public static ServiceResult<T> SuccessfulFactory()
    {
        return new ServiceResult<T>()
        {
            Success = true
        };
    }

    public static ServiceResult<T> FailedFactory(List<string> errors, List<string>? errorCodes = null)
    {
        return new ServiceResult<T>()
        {
            Errors = errors,
            Success = false,
            ErrorCodes = errorCodes
        };
    }

    public static ServiceResult<T> FailedFactory(string error, string? errorCode = null)
    {
        var errors = new List<string> { error };
        var errorCodes = new List<string>();
        if (errorCode is not null)
        {
            errorCodes.Add(errorCode);
        }

        return new ServiceResult<T>()
        {
            Errors = errors,
            Success = false,
            ErrorCodes = errorCodes
        };
    }
}