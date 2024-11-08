namespace ProjektGrupowy.API.Utils;

/// <summary>
/// Represents a result of an operation that can either be successful or failed.
/// </summary>
/// <typeparam name="TOption"></typeparam>
public class Optional<TOption>
{
    private readonly TOption _value;
    private readonly string _error;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Optional(TOption value, string error, bool isSuccess)
    {
        _value = value;
        _error = error;
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Creates a successful result. The value cannot be null.
    /// </summary>
    /// <param name="value">Value returned from an operation</param>
    /// <returns>A successful result containing the <typeparamref name="TOption"/>.</returns>
    /// <exception cref="ArgumentNullException">If value is null.</exception>
    public static Optional<TOption> Success(TOption value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Success value cannot be null");

        return new Optional<TOption>(value, null, true);
    }

    /// <summary>
    /// Creates a failed result. The error cannot be null.
    /// </summary>
    /// <param name="error">Error message describing the failure</param>
    /// <returns>A failed result</returns>
    /// <exception cref="ArgumentNullException">If error is null.</exception>
    public static Optional<TOption> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentNullException(nameof(error), "Error message cannot be null or empty");

        return new Optional<TOption>(default, error, false);
    }

    /// <summary>
    /// Gets the value from a successful result. Will throw an exception if the result is a failure.
    /// </summary>
    /// <returns>The value of the result.</returns>
    /// <exception cref="InvalidOperationException">If the result is a failure.</exception>
    public TOption GetValueOrThrow()
    {
        if (IsFailure)
            throw new InvalidOperationException("Cannot get value from a failed result");
        return _value;
    }

    /// <summary>
    /// Gets the error from a failed result. Will throw an exception if the result is a success.
    /// </summary>
    /// <returns>The error of the result.</returns>
    /// <exception cref="InvalidOperationException">If the result is a success</exception>
    public string GetErrorOrThrow()
    {
        if (IsSuccess)
            throw new InvalidOperationException("Cannot get error from a successful result");
        return _error;
    }

    public override string ToString() => IsSuccess ? $"Success({_value})" : $"Failure({_error})";
}