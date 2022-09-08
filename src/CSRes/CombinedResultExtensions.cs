namespace CSRes;

public static class ThreeUnitResultsExtensions
{
    // TODO Do
}

public static class ThreeResultsExtensions
{
    // TODO Do
}

public static class TwoUnitResultsExtensions
{
    // IResult<T> cannot be used without creating a new object every time...
    public static Result<(T1, T2)> Where<T1, T2>(this Result<(T1, T2)> result, Func<T1, T2, bool> filter) => result switch
    {
        ISuccess<(T1, T2)> s => filter(s.Value.Item1, s.Value.Item2) ? result : Result<(T1, T2)>.Failure,
        IFailure => result,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #region Touch (value/error)

    // TODO Rename to "TouchValue" (to be aligned with collection extension methods)
    public static Result<(T1, T2)> Touch<T1, T2>(this Result<(T1, T2)> result, Action<T1, T2> touch)
    {
        if (result is ISuccess<(T1, T2)> s)
            touch(s.Value.Item1, s.Value.Item2);

        return result;
    }

    public static async Task<Result<(T1, T2)>> Touch<T1, T2>(this Result<(T1, T2)> result, Func<T1, T2, Task> touch)
    {
        if (result is ISuccess<(T1, T2)> s)
            await touch(s.Value.Item1, s.Value.Item2);

        return result;
    }

    // TouchError() is the same as for IResult<T> (unit result)

    #endregion

    #region Map : SelectValue()

    // TODO Rename to "SelectValue" (to be aligned with collection extension methods)
    public static Result<TResult> Select<T1, T2, TResult>(this IResult<(T1, T2)> result,
        Func<T1, T2, TResult> map) => result switch
    {
        ISuccess<(T1, T2)> s => map(s.Value.Item1, s.Value.Item2),
        IFailure => Result<TResult>.Failure,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult>> Select<T1, T2, TResult>(this IResult<(T1, T2)> result,
        Func<T1, T2, Task<TResult>> map) => result switch
    {
        ISuccess<(T1, T2)> s => map(s.Value.Item1, s.Value.Item2).Then(Result.From),
        IFailure => Result<TResult>.FailureTask,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Bind : BindValue()

    // TODO Rename to "BindValue" (to be aligned with collection extension methods)
    public static Result<TResult> Bind<T1, T2, TResult>(this IResult<(T1, T2)> result,
        Func<T1, T2, Result<TResult>> bind) => result switch
    {
        ISuccess<(T1, T2)> s => bind(s.Value.Item1, s.Value.Item2),
        IFailure => Result<TResult>.Failure,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult>> Bind<T1, T2, TResult>(
        this IResult<(T1, T2)> result, Func<T1, T2, Task<Result<TResult>>> bind) => result switch
    {
        ISuccess<(T1, T2)> s => bind(s.Value.Item1, s.Value.Item2),
        IFailure => Result<TResult>.FailureTask,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion
}

public static class TwoResultsExtensions
{
    #region Touch (value/error)

    // TODO Rename to "TouchValue" (to be aligned with collection extension methods)
    public static Result<(T1, T2), TError> Touch<T1, T2, TError>(this Result<(T1, T2), TError> result, Action<T1, T2> touch)
    {
        if (result is ISuccess<(T1, T2)> s)
            touch(s.Value.Item1, s.Value.Item2);

        return result;
    }

    public static async Task<Result<(T1, T2), TError>> Touch<T1, T2, TError>(this Result<(T1, T2), TError> result, Func<T1, T2, Task> touch)
    {
        if (result is ISuccess<(T1, T2)> s)
            await touch(s.Value.Item1, s.Value.Item2);

        return result;
    }

    // TouchError() is the same as for IResult<T, TError>

    #endregion

    #region Map : SelectValue()

    // TODO Rename to "SelectValue" (to be aligned with collection extension methods)
    public static Result<TResult, TError> Select<T1, T2, TError, TResult>(
        this IResult<(T1, T2), TError> result, Func<T1, T2, TResult> map) => result switch
    {
        ISuccess<(T1, T2)> s => map(s.Value.Item1, s.Value.Item2),
        IFailure<TError> f => f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TError>> Select<T1, T2, TError, TResult>(this IResult<(T1, T2), TError> result,
        Func<T1, T2, Task<TResult>> map) => result switch
    {
        ISuccess<(T1, T2)> s => map(s.Value.Item1, s.Value.Item2).Then(Result.From<TResult, TError>),
        IFailure<TError> f => Result.AsyncFrom<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Map : TrySelectValue()

    public static Result<TResult, TError> TrySelect<T1, T2, TError, TResult>(this IResult<(T1, T2), TError> result,
        Func<T1, T2, TResult> map, Func<Exception, T1, T2, TError> mapError) => result switch
    {
        ISuccess<(T1, T2)> s => map.Try(mapError).Invoke(s.Value.Item1, s.Value.Item2),
        IFailure<TError> f => Result.From<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TError>> TrySelect<T1, T2, TError, TResult>(this IResult<(T1, T2), TError> result,
        Func<T1, T2, Task<TResult>> map, Func<Exception, T1, T2, TError> mapError) => result switch
    {
        ISuccess<(T1, T2)> s => map.Try(mapError).Invoke(s.Value.Item1, s.Value.Item2),
        IFailure<TError> f => Result.AsyncFrom<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Bind : BindValue()

    // TODO Rename to "BindValue" (to be aligned with collection extension methods)
    public static Result<TResult, TResultError> Bind<T1, T2, TError, TResult, TResultError>(
        this IResult<(T1, T2), TError> result, Func<T1, T2, Result<TResult, TResultError>> bind)
        where TError : TResultError => result switch
    {
        ISuccess<(T1, T2)> s => bind(s.Value.Item1, s.Value.Item2),
        IFailure<TError> failure => failure.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TResultError>> Bind<T1, T2, TError, TResult, TResultError>(
        this IResult<(T1, T2), TError> result, Func<T1, T2, Task<Result<TResult, TResultError>>> bind)
        where TError : TResultError => result switch
    {
        ISuccess<(T1, T2)> s => bind(s.Value.Item1, s.Value.Item2),
        IFailure<TError> f => Result.AsyncFrom<TResult, TResultError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Bind : TryBindValue()

    public static Result<TResult, TResultError> TryBind<T1, T2, TError, TResult, TResultError>(
        this IResult<(T1, T2), TError> result, Func<T1, T2, Result<TResult, TResultError>> bind,
        Func<Exception, T1, T2, TResultError> mapError) where TError : TResultError =>
        result.Bind(v => bind.Try(mapError).Invoke(v.Item1, v.Item2).Unwrap());

    public static Task<Result<TResult, TResultError>> TryBind<T1, T2, TError, TResult, TResultError>(
        this IResult<(T1, T2), TError> result, Func<T1, T2, Task<Result<TResult, TResultError>>> bind,
        Func<Exception, T1, T2, TResultError> mapError) where TError : TResultError =>
        result.Bind(v => bind.Try(mapError).Invoke(v.Item1, v.Item2).Unwrap());

    #endregion
}
