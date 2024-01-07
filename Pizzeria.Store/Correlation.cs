namespace Pizzeria.Store;

public static class Correlation
{
    public const string HeaderName = "X-Correlation-ID";

    public static readonly string[] RequestHeaders = { HeaderName };
}
