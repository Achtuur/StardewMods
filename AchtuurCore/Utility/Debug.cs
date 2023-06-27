using System;

namespace AchtuurCore.Utility;

public static class Debug
{
    /// <summary>
    /// Execute a method only if in debug mode, so you never have to deal with <c>#if DEBUG</c> automatic indents in visual studio :)
    /// </summary>
    /// <param name="func"></param>
    public static void DebugOnlyExecute(Action func)
    {
#if DEBUG
        func.Invoke();
#endif
    }
}
