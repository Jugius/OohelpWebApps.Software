using System;

namespace SoftwareManager.Services;
public readonly struct ValueResult<TValue>
{
    public TValue Value { get; }
    public Exception Error { get; }
    public bool IsSuccess { get; }
    private ValueResult(TValue value)
    {
        Value = value;
        IsSuccess = true;
        Error = default;
    }
    public ValueResult(Exception error)
    {
        Error = error;
        IsSuccess = false;
        Value = default;
    }
    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Exception, TResult> failure) =>
        IsSuccess ? success(Value) : failure(Error);

    public static implicit operator ValueResult<TValue>(TValue value) => new ValueResult<TValue>(value);
    public static implicit operator ValueResult<TValue>(Exception error) => new ValueResult<TValue>(error);
}
