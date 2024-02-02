namespace DigitalMarketplace.Core.DTOs;
public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }

    public ServiceResponse<T> Succeed(T? data) 
        => new()
        {
            Data = data,
            Success = true
        };

    public ServiceResponse<T> Failed(T? data, string error) 
        => new()
        {
            Data = data,
            Error = error,
            Success = false
        };
}
