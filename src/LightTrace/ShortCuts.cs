using System;
using System.Threading.Tasks;

namespace LightTrace;

public static class ShortCuts
{
    public static T _T<T>(string name, Func<T> func)
    {
        using var tracer = new Tracer(name);
        return func();
    }

    public static async Task<T> _T<T>(string name, Func<Task<T>> func)
    {
        using var tracer = new Tracer(name);
        return await func();
    }

    public static void _T(string name, Action func)
    {
        using var tracer = new Tracer(name);
        func();
    }

    public static async Task _T(string name, Func<Task> func)
    {
        using var tracer = new Tracer(name);
        await func();
    }
}
