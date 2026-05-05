namespace GreenTrace.Server.Common;

/// <summary>
/// Standard response wrapper for all mutations.
/// Used with [Mutation&lt;CallResult&gt;] attribute in CAI pattern.
/// </summary>
public record CallResult<T>
{
    public bool success { get; set; }
    public string? message { get; set; }
    public T? result { get; set; }
    public int code { get; set; }

    public static CallResult<T> Ok(string message) => new() { success = true, message = message };
    public static CallResult<T> Ok(T result, string message, int code = 200) => new() { success = true, message = message, result = result, code = code };
    public static CallResult<T> Error(string message, int code = 400) => new() { success = false, message = message, code = code };
    public static CallResult<T> NotAuthenticated(string message = "Not authenticated. Please login and try again") => Error(message, 401);
}

public record CallResult : CallResult<object>
{
    public static CallResult Ok(string message, int code = 200) => new() { success = true, message = message, code = code };
    public static new CallResult Error(string message, int code = 400) => new() { success = false, message = message, code = code };
    public static CallResult NotPermitted() => Error("Not Permitted", 403);
    public static new CallResult NotAuthenticated() => Error("Not authenticated", 401);
}
