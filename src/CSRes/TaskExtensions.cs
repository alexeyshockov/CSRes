namespace CSRes;

internal static class TaskExtensions
{
    public static Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, TResult> map) =>
        task.ContinueWith(completed => map(completed.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, TResult> map,
        Func<Exception, TResult> mapError) =>
        task.Then(map).OnException(mapError);

    public static Task<Result<TResult, TResultError>> ThenResult<T, TResult, TResultError>(this Task<T> task,
        Func<T, TResult> map, Func<Exception, TResultError> mapError) =>
        task.Then(r => Result.From<TResult, TResultError>(map(r)))
            .OnException(e => Result.From<TResult, TResultError>(mapError(e)));

    public static Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, Task<TResult>> map) => task
        .ContinueWith(completed => map(completed.Result), TaskContinuationOptions.OnlyOnRanToCompletion)
        .Unwrap();

    // TODO Check that is can be used...
    public static Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, Task<TResult>> map,
        Func<Exception, TResult> mapError) =>
        task.Then(map).OnException(mapError);

    // TODO Check if the resulted Task is not "Fault", but "Completed"
    public static Task<TResult> OnException<T, TResult>(this Task<T> task, Func<Exception, TResult> map) => task
        .ContinueWith(completed => map(completed.Exception!.InnerException!), TaskContinuationOptions.OnlyOnFaulted);
}
