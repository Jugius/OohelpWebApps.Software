namespace OohelpWebApps.Software.Updater.Models;
internal class OperationResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Exception Error { get; }
    protected OperationResult()
    { 
        IsSuccess = false;
        Error = default;
    }
    protected OperationResult(Exception error)
    {        
        IsSuccess = false;
        Error = error;
    }
    public static OperationResult Success() => new OperationResult();
    public static OperationResult Failure(Exception error) => new OperationResult(error);
}
internal class OperationResult<TValue> : OperationResult
{
    public TValue Value { get; }

    private OperationResult(TValue value) : base()
    {
        Value = value;
    }
    private OperationResult(Exception error) : base(error)
    {        
        Value = default;
    }
    public static implicit operator OperationResult<TValue>(TValue value) => new(value);
    public static implicit operator OperationResult<TValue>(Exception error) => new(error);
}
