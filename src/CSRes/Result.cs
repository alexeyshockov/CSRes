namespace CSRes;

public interface IResult
{
}

public interface IResult<out T> : IResult
{
}

public interface IResult<out T, out TError> : IResult<T>
{
}

public interface ISuccess : IResult
{
}

public interface ISuccess<out T> : ISuccess, IResult<T>
{
    T Value { get; }
}

public interface ISuccess<out T, out TError> : ISuccess<T>, IResult<T, TError>
{
}

public interface IFailure : IResult
{
}

public interface IFailure<out TError> : IFailure
{
    TError Error { get; }
}

public interface IFailure<out T, out TError> : IFailure<TError>, IResult<T, TError>
{
}

internal abstract record JustResult : IResult
{
    internal sealed record SuccessImpl : JustResult, ISuccess;
    internal sealed record FailureImpl : JustResult, IFailure;
}

/// <summary>
/// С# lacks unit type (as it's called in Haskell), that why a whole separate class is needed in addition to
/// Result{T, TError}.
/// </summary>
/// <typeparam name="T">Result's value.</typeparam>
public abstract record Result<T> : IResult<T> // UnitResult
{
    public static readonly Result<T> Failure = new FailureImpl();
    public static readonly Task<Result<T>> FailureTask = Task.FromResult(Failure);

    public sealed record Success(T Value) : Result<T>, ISuccess<T>;

    private sealed record FailureImpl : Result<T>, IFailure;

    public static implicit operator Result<T>(T value) => new Success(value);
}

public abstract record Result<T, TError> : Result<T>, IResult<T, TError>
{
    public new sealed record Success(T Value) : Result<T, TError>, ISuccess<T, TError>;

    public new sealed record Failure(TError Error) : Result<T, TError>, IFailure<T, TError>;

    public static implicit operator Result<T, TError>(T value) => new Success(value);

    public static implicit operator Result<T, TError>(TError error) => new Failure(error);
}

public static partial class Result // JustResult
{
    internal static readonly Func<Exception, Exception> IdErrorHandler = e => e;

    public static readonly IResult Success = new JustResult.SuccessImpl();
    public static readonly IResult Failure = new JustResult.FailureImpl();

    #region Unwrap() (lift?)

//    public static IResult<T> Unwrap<T>(this IResult<IResult<T>> result) => result switch
//    {
//        ISuccess<IResult<T>> s => s.Value,
//        IFailure => Result<T>.Failure,
//        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
//    };

    public static Result<T> Unwrap<T>(this IResult<Result<T>> result) => result.Bind(v => v);

//    public static IResult<T, TError> Unwrap<T, TError>(this IResult<IResult<T, TError>, TError> result) => result switch
//    {
//        ISuccess<IResult<T, TError>> s => s.Value,
//        IFailure<TError> f => Result.From<T, TError>(f.Error),
//        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
//    };

    public static Result<T, TResultError> Unwrap<T, TError, TResultError>(
        this IResult<Result<T, TResultError>, TError> result) where TError : TResultError =>
        result.Bind(v => v);

    #endregion

    #region Try()

    public static Func<Result<T, Exception>> Try<T>(this Func<T> task) => Try(task, IdErrorHandler);
    public static Func<Result<T, TError>> Try<T, TError>(this Func<T> task, Func<Exception, TError> mapError) => () =>
    {
        try
        {
            return task();
        }
        catch (Exception e)
        {
            return mapError(e);
        }
    };

    public static Func<T, Result<TResult, Exception>> Try<T, TResult>(this Func<T, TResult> task) =>
        Try(task, (e, _) => e);
    public static Func<T, Result<TResult, TError>> Try<T, TResult, TError>(
        this Func<T, TResult> task, Func<Exception, T, TError> mapError) => a =>
    {
        try
        {
            return task(a);
        }
        catch (Exception e)
        {
            return mapError(e, a);
        }
    };

    public static Func<T1, T2, Result<TResult, Exception>> Try<T1, T2, TResult>(this Func<T1, T2, TResult> task) =>
        Try(task, (e, _, _) => e);
    public static Func<T1, T2, Result<TResult, TError>> Try<T1, T2, TResult, TError>(
        this Func<T1, T2, TResult> task, Func<Exception, T1, T2, TError> mapError) => (a1, a2) =>
    {
        try
        {
            return task(a1, a2);
        }
        catch (Exception e)
        {
            return mapError(e, a1, a2);
        }
    };

    public static Func<T1, T2, T3, Result<TResult, Exception>> Try<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> task) =>
        Try(task, (e, _, _, _) => e);
    public static Func<T1, T2, T3, Result<TResult, TError>> Try<T1, T2, T3, TResult, TError>(
        this Func<T1, T2, T3, TResult> task, Func<Exception, T1, T2, T3, TError> mapError) => (a1, a2, a3) =>
    {
        try
        {
            return task(a1, a2, a3);
        }
        catch (Exception e)
        {
            return mapError(e, a1, a2, a3);
        }
    };

    #endregion

    #region Combine & Zip

    public static IResult Combine<T>(params T[] results) where T : IResult => results.Any(x => x is IFailure)
        ? Failure
        : Success;

    public static IResult Combine<T, TR>(params TR[] results) where TR : IResult<T> => results.Any(x => x is IFailure)
        ? Failure
        : Success;

    public static Result<(T1, T2)> Zip<T1, T2>(IResult<T1> r1, IResult<T2> r2) => (r1, r2) switch
    {
        (ISuccess<T1> s1, ISuccess<T2> s2) => (s1.Value, s2.Value),
        _ => Result<(T1, T2)>.Failure
    };

    public static Result<(T1, T2, T3)> Zip<T1, T2, T3>(IResult<T1> r1, IResult<T2> r2, IResult<T3> r3) => (r1, r2, r3) switch
    {
        (ISuccess<T1> s1, ISuccess<T2> s2, ISuccess<T3> s3) => (s1.Value, s2.Value, s3.Value),
        _ => Result<(T1, T2, T3)>.Failure
    };

    #endregion

    #region From()

    public static Result<T> From<T>(T value) => value;
    public static Result<T> From<T>(IResult<T> result) => result switch
    {
        Result<T> r => r,
        ISuccess<T> s => s.Value,
        IFailure => Result<T>.Failure,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Result<T, TError> From<T, TError>(T value) => value;
    public static Result<T, TError> From<T, TError>(TError error) => error;
    public static Result<T, TError> From<T, TError>(IResult<T, TError> result) => result switch
    {
        Result<T, TError> r => r,
        ISuccess<T> s => s.Value,
        IFailure<TError> f => f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<T>> AsyncFrom<T>(T value) => Task.FromResult(From(value));

    public static Task<Result<T, TError>> AsyncFrom<T, TError>(T value) => Task.FromResult(From<T, TError>(value));
    public static Task<Result<T, TError>> AsyncFrom<T, TError>(TError error) => Task.FromResult(From<T, TError>(error));

    #endregion
}
