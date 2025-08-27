using System.Collections.Generic;

namespace LightTrace.Extensions;

internal static class StackExtensions
{
    internal static T PeekOrDefault<T>(this Stack<T> stack) => 
        stack.Count == 0 ? default : stack.Peek();
}