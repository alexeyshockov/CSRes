namespace CSRes;

public static class UnitResultExtensions
{
    public static T Get<T>(this IResult<T> result) => result switch
    {
        ISuccess<T> s => s.Value,
        IFailure => throw new IndexOutOfRangeException(),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #region Logical operations

    public static Result<(T1, T2)> And<T1, T2>(this IResult<T1> r1, IResult<T2> r2) => Result.Zip(r1, r2);

    public static Result<T> Or<T>(this Result<T> r1, Result<T> r2) => (r1, r2) switch
    {
        (ISuccess<T>, _) => r1,
        (_, ISuccess<T>) => r2,
        _ => Result<T>.Failure
    };

    public static Result<T> Xor<T>(this Result<T> r1, Result<T> r2) => (r1, r2) switch
    {
        (ISuccess<T>, IFailure) => r1,
        (IFailure, ISuccess<T>) => r2,
        _ => Result<T>.Failure
    };

    #endregion

    // IResult<T> cannot be used without creating a new object every time...
    public static Result<T> Where<T>(this Result<T> result, Predicate<T> filter) => result switch
    {
        ISuccess<T> s => filter(s.Value) ? result : Result<T>.Failure,
        IFailure => result,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #region Touch (value/error)

    // TODO Rename to "TouchValue" (to be aligned with collection extension methods)
    public static Result<T> Touch<T>(this Result<T> result, Action<T> touch)
    {
        if (result is ISuccess<T> s)
            touch(s.Value);

        return result;
    }

    public static async Task<Result<T>> Touch<T>(this Result<T> result, Func<T, Task> touch)
    {
        if (result is ISuccess<T> s)
            await touch(s.Value);

        return result;
    }

    public static Result<T> TouchError<T>(this Result<T> result, Action touch)
    {
        if (result is IFailure)
            touch();

        return result;
    }
    public static async Task<Result<T>> TouchError<T>(this Result<T> result, Func<Task> touch)
    {
        if (result is IFailure)
            await touch();

        return result;
    }

    #endregion

    #region Map : SelectValue()

    // TODO Rename to "SelectValue" (to be aligned with collection extension methods)
    public static Result<TResult> Select<T, TResult>(this IResult<T> result,
        Func<T, TResult> map) => result switch
    {
        ISuccess<T> s => map(s.Value),
        IFailure f => Result<TResult>.Failure,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult>> Select<T, TResult>(this IResult<T> result,
        Func<T, Task<TResult>> map) => result switch
    {
        ISuccess<T> s => map(s.Value).Then(Result.From),
        IFailure => Result<TResult>.FailureTask,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Bind : BindValue()

    // TODO Rename to "BindValue" (to be aligned with collection extension methods)
    public static Result<TResult> Bind<T, TResult>(this IResult<T> result,
        Func<T, Result<TResult>> bind) => result switch
    {
        ISuccess<T> s => bind(s.Value),
        IFailure => Result<TResult>.Failure,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult>> Bind<T, TResult>(
        this IResult<T> result, Func<T, Task<Result<TResult>>> bind) => result switch
    {
        ISuccess<T> s => bind(s.Value),
        IFailure => Result<TResult>.FailureTask,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    // Try...() is not possible, because we cannot store an error...
}
