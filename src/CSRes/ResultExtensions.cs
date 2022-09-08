namespace CSRes;

public static partial class ResultExtensions
{
    // TODO Get() where TError is not an exception (wrap)
    public static T Get<T, TError>(this IResult<T, TError> result) where TError : Exception => result switch
    {
        ISuccess<T> s => s.Value,
        IFailure<TError> f => throw f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #region Logical operations

    public static Result<TError, T> Invert<T, TError>(this IResult<T, TError> result) => result switch
    {
        ISuccess<T> s => s.Value,
        IFailure<TError> f => f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Touch (value/error)

    // TODO Rename to "TouchValue" (to be aligned with collection extension methods)
    public static Result<T, TError> Touch<T, TError>(this Result<T, TError> result, Action<T> touch)
    {
        if (result is ISuccess<T> s)
            touch(s.Value);

        return result;
    }

    public static async Task<Result<T, TError>> Touch<T, TError>(this Result<T, TError> result, Func<T, Task> touch)
    {
        if (result is ISuccess<T> s)
            await touch(s.Value);

        return result;
    }

    public static Result<T, TError> TouchError<T, TError>(this Result<T, TError> result, Action<TError> touch)
    {
        if (result is IFailure<TError> f)
            touch(f.Error);

        return result;
    }

    public static async Task<Result<T, TError>> TouchError<T, TError>(this Result<T, TError> result,
        Func<TError, Task> touch)
    {
        if (result is IFailure<TError> f)
            await touch(f.Error);

        return result;
    }

    #endregion

    #region Map : SelectValue()

    // TODO Rename to "SelectValue" (to be aligned with collection extension methods)
    public static Result<TResult, TError> Select<T, TError, TResult>(this IResult<T, TError> result,
        Func<T, TResult> map) => result switch
    {
        ISuccess<T> s => map(s.Value),
        IFailure<TError> f => f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TError>> Select<T, TError, TResult>(this IResult<T, TError> result,
        Func<T, Task<TResult>> map) => result switch
    {
        ISuccess<T> s => map(s.Value).Then(Result.From<TResult, TError>),
        IFailure<TError> f => Result.AsyncFrom<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Map : TrySelectValue()

    public static Result<TResult, TError> TrySelect<T, TError, TResult>(this IResult<T, TError> result,
        Func<T, TResult> map, Func<Exception, T, TError> mapError) => result switch
    {
        ISuccess<T> s => map.Try(mapError).Invoke(s.Value),
        IFailure<TError> f => Result.From<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TError>> TrySelect<T, TError, TResult>(this IResult<T, TError> result,
        Func<T, Task<TResult>> map, Func<Exception, T, TError> mapError) => result switch
    {
        ISuccess<T> s => map.Try(mapError).Invoke(s.Value),
        IFailure<TError> f => Result.AsyncFrom<TResult, TError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    public static Result<T, TResultError> SelectError<T, TError, TResultError>(this IResult<T, TError> result,
        Func<TError, TResultError> map) => result switch
    {
        ISuccess<T> s => s.Value,
        IFailure<TError> f => map(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #region Bind : BindValue()

    // TODO Rename to "BindValue" (to be aligned with collection extension methods)
    public static Result<TResult, TResultError> Bind<T, TError, TResult, TResultError>(this IResult<T, TError> result,
        Func<T, Result<TResult, TResultError>> bind) where TError : TResultError => result switch
    {
        ISuccess<T> s => bind(s.Value),
        IFailure<TError> f => f.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    public static Task<Result<TResult, TResultError>> Bind<T, TError, TResult, TResultError>(
        this IResult<T, TError> result, Func<T, Task<Result<TResult, TResultError>>> bind)
        where TError : TResultError => result switch
    {
        ISuccess<T> s => bind(s.Value),
        IFailure<TError> f => Result.AsyncFrom<TResult, TResultError>(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    #endregion

    #region Bind : TryBindValue()

    public static Result<TResult, TResultError> TryBind<T, TError, TResult, TResultError>(
        this IResult<T, TError> result, Func<T, Result<TResult, TResultError>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        result.Bind(v => bind.Try(mapError).Invoke(v).Unwrap());

    public static Task<Result<TResult, TResultError>> TryBind<T, TError, TResult, TResultError>(
        this IResult<T, TError> result, Func<T, Task<Result<TResult, TResultError>>> bind,
        Func<Exception, T, TResultError> mapError) where TError : TResultError =>
        result.Bind(v => bind.Try(mapError).Invoke(v).Unwrap());

    #endregion

    public static Result<T, TResultError> BindError<T, TError, TResultError>(this IResult<T, TError> result,
        Func<TError, Result<T, TResultError>> bind) => result switch
    {
        ISuccess<T> s => s.Value,
        IFailure<TError> f => bind(f.Error),
        _ => throw new ArgumentOutOfRangeException(nameof(result), "Unknown result type")
    };

    // TODO Support IResult<T> in:
    //  - BindError()
    //  - BindValue()
    // And that's all...
}
