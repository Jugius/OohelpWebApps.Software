using System;

namespace OohelpWebApps.Software.ZipExtractor;
public readonly struct ExtractionResult
{
    public bool IsSuccess { get; }
    public bool Cancelled {  get;  }
    public System.Exception Error { get; }

    private ExtractionResult(bool isSuccess, bool cancelled, Exception error)
    {
        IsSuccess = isSuccess;
        Cancelled = cancelled;
        Error = error;
    }
    public static ExtractionResult Success => new ExtractionResult(true, false, null);
    public static ExtractionResult Cancel => new ExtractionResult(false, true, null);
    public static ExtractionResult FromException(Exception exception) => new ExtractionResult(false, false, exception);
}
