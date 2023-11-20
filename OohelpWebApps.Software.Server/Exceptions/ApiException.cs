namespace OohelpWebApps.Software.Server.Exceptions;
internal class ApiException : Exception
{
    public ExceptionReason Reason { get; }

    public ApiException(ExceptionReason reason) : base(string.Empty)
    {
        Reason = reason;
    }
    public ApiException(ExceptionReason reason, string message) : base(message)
    {
        Reason = reason;
    }
    public static ApiException NotFound() => new ApiException(ExceptionReason.NotFound);
    public static ApiException DatabaseError(string message) => new ApiException(ExceptionReason.DatabaseError, message);
    public static ApiException FileSystemError(string message) => new ApiException(ExceptionReason.FileSystemError, message);
    public static ApiException InvalidRequest(string message) => new ApiException(ExceptionReason.InvalidRequest, message);
}
