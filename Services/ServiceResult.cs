namespace TheNoir.Api.Services;

// Lets services report failures ("email already taken") without throwing
// or leaking HTTP concerns; controllers translate Error into status codes.
public class ServiceResult<T>
{
    public T? Value { get; private init; }
    public string? Error { get; private init; }
    public bool Succeeded => Error is null;

    public static ServiceResult<T> Ok(T value) => new() { Value = value };
    public static ServiceResult<T> Fail(string error) => new() { Error = error };
}
