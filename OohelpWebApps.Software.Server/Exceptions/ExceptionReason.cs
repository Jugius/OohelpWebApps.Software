namespace OohelpWebApps.Software.Server.Exceptions;

internal enum ExceptionReason
{
    InvalidRequest,

    NotFound,
    DatabaseError,
    FileSystemError,

    UnknownError,
}
