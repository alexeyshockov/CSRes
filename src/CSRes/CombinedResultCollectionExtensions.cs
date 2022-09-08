namespace CSRes;

public static class ThreeResultsCollectionExtensions
{
    // TODO Do
}

public static class TwoResultsCollectionExtensions
{
    #region Try

    public static IEnumerable<Result<TResult, Exception>> TrySelect<T1, T2, TResult>(
        this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, TResult> map) =>
        enumerable.Select(x => map.Try().Invoke(x.Item1, x.Item2));

    public static IEnumerable<Result<TResult, TResultError>> TrySelect<T1, T2, TResult, TResultError>(
        this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, TResult> map,
        Func<Exception, T1, T2, TResultError> mapError) =>
        enumerable.Select(x => map.Try(mapError).Invoke(x.Item1, x.Item2));

    public static IEnumerable<Task<Result<TResult, TResultError>>> TrySelect<T1, T2, TResult, TResultError>(
        this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, Task<TResult>> map,
        Func<Exception, T1, T2, TResultError> mapError) =>
        enumerable.Select(x => map.Try(mapError).Invoke(x.Item1, x.Item2));

    #endregion

    #region Touch (value/error)

    public static IEnumerable<Result<(T1, T2), TError>> TouchValues<T1, T2, TError>(this IEnumerable<Result<(T1, T2), TError>> results,
        Action<T1, T2> touch) => results.Select(r => r.Touch(touch));

    public static IEnumerable<Task<Result<(T1, T2), TError>>> TouchValues<T1, T2, TError>(
        this IEnumerable<Result<(T1, T2), TError>> results, Func<T1, T2, Task> touch) => results.Select(r => r.Touch(touch));

    // TouchErrors() is the same

    #endregion

    #region Map (over a collection) : SelectValue()

    public static IEnumerable<Result<TResult, TError>> SelectValues<T1, T2, TError, TResult>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, TResult> map) =>
        results.Select(r => r.Select(map));

    public static IEnumerable<Task<Result<TResult, TError>>> SelectValues<T1, T2, TError, TResult>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Task<TResult>> map) =>
        results.Select(r => r.Select(map));

    #endregion

    #region Map (over a collection) : TrySelectValue()

    public static IEnumerable<Result<TResult, TError>> TrySelectValues<T1, T2, TError, TResult>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, TResult> map,
        Func<Exception, T1, T2, TError> mapError) =>
        results.Select(r => r.TrySelect(map, mapError));

    public static IEnumerable<Task<Result<TResult, TError>>> TrySelectValues<T1, T2, TError, TResult>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Task<TResult>> map,
        Func<Exception, T1, T2, TError> mapError) =>
        results.Select(r => r.TrySelect(map, mapError));

    #endregion

    // SelectErrors() is the same

    #region Bind (over a collection) : Bind()

    public static IEnumerable<Result<TResult, TResultError>> BindValues<T1, T2, TError, TResult, TResultError>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Result<TResult, TResultError>> bind)
        where TError : TResultError =>
        results.Select(r => r.Bind(bind));

    public static IEnumerable<Task<Result<TResult, TResultError>>> BindValues<T1, T2, TError, TResult, TResultError>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Task<Result<TResult, TResultError>>> bind)
        where TError : TResultError =>
        results.Select(r => r.Bind(bind));

    #endregion

    #region Bind (over a collection) : TryBindValue()

    public static IEnumerable<Result<TResult, TResultError>> TryBindValues<T1, T2, TError, TResult, TResultError>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Result<TResult, TResultError>> bind,
        Func<Exception, T1, T2, TResultError> mapError) where TError : TResultError =>
        results.Select(r => r.TryBind(bind, mapError));

    public static IEnumerable<Task<Result<TResult, TResultError>>> TryBindValues<T1, T2, TError, TResult, TResultError>(
        this IEnumerable<IResult<(T1, T2), TError>> results, Func<T1, T2, Task<Result<TResult, TResultError>>> bind,
        Func<Exception, T1, T2, TResultError> mapError) where TError : TResultError =>
        results.Select(r => r.TryBind(bind, mapError));

    #endregion

    // BindErrors() is the same
}
