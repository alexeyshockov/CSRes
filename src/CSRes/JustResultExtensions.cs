namespace CSRes;

public static class JustResultExtensions
{
    #region Logical operations

    public static IResult And(this IResult r1, IResult r2) =>
        r1 is ISuccess && r2 is ISuccess ? Result.Success : Result.Failure;

    public static IResult Or(this IResult r1, IResult r2) =>
        r1 is ISuccess || r2 is ISuccess ? Result.Success : Result.Failure;

    public static IResult Xor(this IResult r1, IResult r2) =>
        r1 is ISuccess ^ r2 is ISuccess ? Result.Success : Result.Failure;

    #endregion
}
