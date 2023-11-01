namespace Domain.Common;

/// <summary>
/// Represents the result of a service method call
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public virtual T? Data { get; set; }

    public List<string> Errors { get; set; } = new List<string>();
    // TODO: Add Error Codes
}