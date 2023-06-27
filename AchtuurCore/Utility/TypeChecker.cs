namespace AchtuurCore.Utility;

public static class TypeChecker
{
    public static bool isType<T>(object obj)
    {
        return obj is not null && obj.GetType() == typeof(T);
    }
}
