namespace Api.Helpers;

public static class Extensions
{
    public static string LikeConcat(this string str) => string.Concat("%", str, "%");
}
