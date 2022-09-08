namespace CSRes;

/// <summary>
/// Operations over a collection of results.
/// </summary>
public static partial class UnitResultCollectionExtensions
{
    public static IEnumerable<IFailure> OfFailure(this IEnumerable<IResult> results) =>
        results.OfType<IFailure>();

    public static IEnumerable<ISuccess<T>> OfSuccess<T>(this IEnumerable<IResult<T>> results) =>
        results.OfType<ISuccess<T>>();

    public static IEnumerable<TError> OfError<TError>(this IEnumerable<IResult> results) =>
        results.OfType<IFailure<TError>>().Select(f => f.Error);

    public static IEnumerable<T> OfValue<T>(this IEnumerable<IResult<T>> results) =>
        results.OfSuccess().Select(s => s.Value);

    // TODO Other methods
}
