namespace CSRes;

public static partial class Result
{
    #region Unwrap() (lift?)

//    public static Task<IResult<T>> Unwrap<T>(this Task<IResult<IResult<T>>> result) =>
//        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task<Result<T>> Unwrap<T>(this Task<IResult<Result<T>>> result) =>
//        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<Result<T>> Unwrap<T>(this Task<Result<Result<T>>> result) =>
        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task<IResult<T, TError>> Unwrap<T, TError>(this Task<IResult<IResult<T, TError>, TError>> result) =>
//        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task<Result<T, TError>> Unwrap<T, TError>(this Task<IResult<Result<T, TError>, TError>> result) =>
//        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<Result<T, TResultError>> Unwrap<T, TError, TResultError>(
        this Task<Result<Result<T, TResultError>, TError>> result) where TError : TResultError =>
        result.ContinueWith(completed => completed.Result.Unwrap(), TaskContinuationOptions.OnlyOnRanToCompletion);

    #endregion

    #region Try()

    public static Task<Result<T, Exception>> Try<T>(this Task<T> task) => Try(task, IdErrorHandler);

    public static Task<Result<T, TError>> Try<T, TError>(this Task<T> task, Func<Exception, TError> mapError) =>
        task.Then(v => Result.From<T, TError>(v), e => Result.From<T, TError>(mapError(e)));
    // This is how it looked originally...
//    public static async Task<IResult<T, TError>> Try<T, TError>(this Task<T> task, Func<Exception, TError> mapError)
//    {
//        try
//        {
//            return new Success<T, TError>(await task);
//        }
//        catch (OperationCanceledException) when (task.IsCanceled)
//        {
//            // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation
//            // https://stackoverflow.com/a/42158695/322079
//            // https://docs.particular.net/nservicebus/cancellation-and-catching-exceptions#catching-system-operationcanceledexception
//            throw;
//        }
//        catch (Exception e)
//        {
//            return new Failure<T, TError>(mapError(e));
//        }
//    }

    public static Func<T, Task<Result<TResult, Exception>>> Try<T, TResult>(this Func<T, Task<TResult>> task) =>
        task.Try((e, _) => e);

    public static Func<T, Task<Result<TResult, TError>>> Try<T, TResult, TError>(this Func<T, Task<TResult>> task,
        Func<Exception, T, TError> mapError) => a =>
        task(a).ThenResult(v => v, e => mapError(e, a));

    public static Func<T1, T2, Task<Result<TResult, Exception>>> Try<T1, T2, TResult>(this Func<T1, T2, Task<TResult>> task) =>
        task.Try((e, _, _) => e);

    public static Func<T1, T2, Task<Result<TResult, TError>>> Try<T1, T2, TResult, TError>(this Func<T1, T2, Task<TResult>> task,
        Func<Exception, T1, T2, TError> mapError) => (a1, a2) =>
        task(a1, a2).ThenResult(v => v, e => mapError(e, a1, a2));

    public static Func<T1, T2, T3, Task<Result<TResult, Exception>>> Try<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<TResult>> task) =>
        task.Try((e, _, _, _) => e);

    public static Func<T1, T2, T3, Task<Result<TResult, TError>>> Try<T1, T2, T3, TResult, TError>(this Func<T1, T2, T3, Task<TResult>> task,
        Func<Exception, T1, T2, T3, TError> mapError) => (a1, a2, a3) =>
        task(a1, a2, a3).ThenResult(v => v, e => mapError(e, a1, a2, a3));

    #endregion
}
