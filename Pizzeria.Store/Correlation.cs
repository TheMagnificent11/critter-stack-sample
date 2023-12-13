namespace Pizzeria.Store;

public static class Correlation
{
    public const string HeaderName = "X-Correlation-ID";

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
    public static readonly string[] RequestHeaders = [HeaderName];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
}
