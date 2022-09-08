namespace CSRes;

/// <summary>
/// Operations over a collection of async results (a result wrapped in a task).
/// </summary>
public static partial class ResultCollectionExtensions
{
    #region Try

    public static IEnumerable<Task<Result<T, Exception>>> Try<T>(this IEnumerable<Task<T>> tasks) =>
        tasks.Select(Result.Try);
    public static IEnumerable<Task<Result<T, TError>>> Try<T, TError>(this IEnumerable<Task<T>> tasks,
        Func<Exception, TError> mapError) =>
        tasks.Select(t => t.Try(mapError));

    #endregion

//    #region Unwrap() (lift?)
//
//    public static IEnumerable<Task<Result<T, TError>>> Unwrap<T, TError>(
//        this IEnumerable<IResult<Task<T>, TError>> results) => results.Select(Result.Unwrap);
//
//    #endregion

    #region Touch (value/error)

    public static IEnumerable<Task<Result<T, TError>>> TouchValues<T, TError>(this IEnumerable<Task<Result<T, TError>>> results,
        Action<T> touch) => results.Select(t => t.Then(r => r.Touch(touch)));

    public static IEnumerable<Task<Result<T, TError>>> TouchValues<T, TError>(this IEnumerable<Task<Result<T, TError>>> results,
        Func<T, Task> touch) => results.Select(t => t.Then(r => r.Touch(touch)));

    public static IEnumerable<Task<Result<T, TError>>> TouchErrors<T, TError>(this IEnumerable<Task<Result<T, TError>>> results,
        Action<TError> touch) => results.Select(t => t.Then(r => r.TouchError(touch)));

    public static IEnumerable<Task<Result<T, TError>>> TouchErrors<T, TError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<TError, Task> touch) =>
        results.Select(t => t.Then(r => r.TouchError(touch)));

    #endregion

    #region Map (over a collection) : SelectValue()

    public static IEnumerable<Task<Result<TResult, TError>>> SelectValues<T, TError, TResult>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, TResult> map) =>
        results.Select(t => t.Then(r => r.Select(map)));

    public static IEnumerable<Task<Result<TResult, TError>>> SelectValues<T, TError, TResult>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Task<TResult>> map) =>
        results.Select(t => t.Then(r => r.Select(map)));

    #endregion

    #region Map (over a collection) : TrySelectValue()

    public static IEnumerable<Task<Result<TResult, TError>>> TrySelectValues<T, TError, TResult>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, TResult> map,
        Func<Exception, T, TError> mapError) =>
        results.Select(t => t.Then(r => r.TrySelect(map, mapError)));

    public static IEnumerable<Task<Result<TResult, TError>>> TrySelectValues<T, TError, TResult>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Task<TResult>> map,
        Func<Exception, T, TError> mapError) =>
        results.Select(t => t.Then(r => r.TrySelect(map, mapError)));

    #endregion

    public static IEnumerable<Task<Result<T, TResultError>>> SelectErrors<T, TError, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<TError, TResultError> map) =>
        results.Select(t => t.Then(r => r.SelectError(map)));

    #region Bind (over a collection) : Bind()

    public static IEnumerable<Task<Result<TResult, TResultError>>> BindValues<T, TError, TResult, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Result<TResult, TResultError>> bind)
        where TError : TResultError =>
        results.Select(t => t.Then(r => r.Bind(bind)));

    public static IEnumerable<Task<Result<TResult, TResultError>>> BindValues<T, TError, TResult, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Task<Result<TResult, TResultError>>> bind)
        where TError : TResultError =>
        results.Select(t => t.Then(r => r.Bind(bind)));

    #endregion

    #region Bind (over a collection) : TryBindValue()

    public static IEnumerable<Task<Result<TResult, TResultError>>> TryBindValues<T, TError, TResult, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Result<TResult, TResultError>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        results.Select(t => t.Then(r => r.TryBind(bind, mapError)));

    public static IEnumerable<Task<Result<TResult, TResultError>>> TryBindValues<T, TError, TResult, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<T, Task<Result<TResult, TResultError>>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        results.Select(t => t.Then(r => r.TryBind(bind, mapError)));

    #endregion

    public static IEnumerable<Task<Result<T, TResultError>>> BindErrors<T, TError, TResultError>(
        this IEnumerable<Task<Result<T, TError>>> results, Func<TError, Result<T, TResultError>> bind) =>
        results.Select(t => t.Then(r => r.BindError(bind)));
}
