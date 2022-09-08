namespace CSRes;

/// <summary>
/// Operations over a collection of results.
/// </summary>
public static partial class ResultCollectionExtensions
{
    public static IEnumerable<IFailure<T, TError>> OfFailure<T, TError>(this IEnumerable<IResult<T, TError>> results) =>
        results.OfType<IFailure<T, TError>>();

    public static IEnumerable<ISuccess<T, TError>> OfSuccess<T, TError>(this IEnumerable<IResult<T, TError>> results) =>
        results.OfType<ISuccess<T, TError>>();

    public static IEnumerable<TError> OfError<T, TError>(this IEnumerable<IResult<T, TError>> results) =>
        results.OfFailure().Select(f => f.Error);

    public static IEnumerable<T> OfValue<T, TError>(this IEnumerable<IResult<T, TError>> results) =>
        results.OfSuccess().Select(s => s.Value);

    #region Try

    public static IEnumerable<Result<TResult, Exception>> TrySelect<T, TResult>(
        this IEnumerable<T> enumerable, Func<T, TResult> map) =>
        enumerable.Select(map.Try());

    public static IEnumerable<Result<TResult, TResultError>> TrySelect<T, TResult, TResultError>(
        this IEnumerable<T> enumerable, Func<T, TResult> map, Func<Exception, T, TResultError> mapError) =>
        enumerable.Select(map.Try(mapError));

    public static IEnumerable<Task<Result<TResult, TResultError>>> TrySelect<T, TResult, TResultError>(
        this IEnumerable<T> enumerable, Func<T, Task<TResult>> map, Func<Exception, T, TResultError> mapError) =>
        enumerable.Select(map.Try(mapError));

    #endregion

    #region Touch (value/error)

    public static IEnumerable<Result<T, TError>> TouchValues<T, TError>(this IEnumerable<Result<T, TError>> results,
        Action<T> touch) => results.Select(r => r.Touch(touch));

    public static IEnumerable<Task<Result<T, TError>>> TouchValues<T, TError>(
        this IEnumerable<Result<T, TError>> results, Func<T, Task> touch) => results.Select(r => r.Touch(touch));

    public static IEnumerable<Result<T, TError>> TouchErrors<T, TError>(this IEnumerable<Result<T, TError>> results,
        Action<TError> touch) => results.Select(r => r.TouchError(touch));

    public static IEnumerable<Task<Result<T, TError>>> TouchErrors<T, TError>(
        this IEnumerable<Result<T, TError>> results, Func<TError, Task> touch) =>
        results.Select(r => r.TouchError(touch));

    #endregion

    #region Map (over a collection) : SelectValue()

    public static IEnumerable<Result<TResult, TError>> SelectValues<T, TError, TResult>(
        this IEnumerable<IResult<T, TError>> results, Func<T, TResult> map) =>
        results.Select(r => r.Select(map));

    public static IEnumerable<Task<Result<TResult, TError>>> SelectValues<T, TError, TResult>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Task<TResult>> map) =>
        results.Select(r => r.Select(map));

    #endregion

    #region Map (over a collection) : TrySelectValue()

    public static IEnumerable<Result<TResult, TError>> TrySelectValues<T, TError, TResult>(
        this IEnumerable<IResult<T, TError>> results, Func<T, TResult> map,
        Func<Exception, T, TError> mapError) =>
        results.Select(r => r.TrySelect(map, mapError));

    public static IEnumerable<Task<Result<TResult, TError>>> TrySelectValues<T, TError, TResult>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Task<TResult>> map,
        Func<Exception, T, TError> mapError) =>
        results.Select(r => r.TrySelect(map, mapError));

    #endregion

    public static IEnumerable<Result<T, TResultError>> SelectErrors<T, TError, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<TError, TResultError> map) =>
        results.Select(r => r.SelectError(map));

    #region Bind (over a collection) : Bind()

    public static IEnumerable<Result<TResult, TResultError>> BindValues<T, TError, TResult, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Result<TResult, TResultError>> bind)
        where TError : TResultError =>
        results.Select(r => r.Bind(bind));

    public static IEnumerable<Task<Result<TResult, TResultError>>> BindValues<T, TError, TResult, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Task<Result<TResult, TResultError>>> bind)
        where TError : TResultError =>
        results.Select(r => r.Bind(bind));

    #endregion

    #region Bind (over a collection) : TryBindValue()

    public static IEnumerable<Result<TResult, TResultError>> TryBindValues<T, TError, TResult, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Result<TResult, TResultError>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        results.Select(r => r.TryBind(bind, mapError));

    public static IEnumerable<Task<Result<TResult, TResultError>>> TryBindValues<T, TError, TResult, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<T, Task<Result<TResult, TResultError>>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        results.Select(r => r.TryBind(bind, mapError));

    #endregion

    public static IEnumerable<Result<T, TResultError>> BindErrors<T, TError, TResultError>(
        this IEnumerable<IResult<T, TError>> results, Func<TError, Result<T, TResultError>> bind) =>
        results.Select(r => r.BindError(bind));
}
