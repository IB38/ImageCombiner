using System;
using System.Threading.Tasks;
using Avalonia.Input;
using CodeJam;

namespace ImageCombiner.App;

public static class ElementHelper
{
    public static async Task WrapWithDisable(object? sender, Func<Task> asyncFunctionBody)
    {
        Code.NotNull(sender, nameof(sender));
        var senderElement = sender as InputElement ??
                            throw new InvalidOperationException(
                                $"Unexpected sender type {sender?.GetType().Name ?? "null"}");

        var initiallyEnabled = senderElement.IsEnabled;
        senderElement.IsEnabled = false;
        
        try
        {
            await asyncFunctionBody();
        }
        finally
        {
            senderElement.IsEnabled = initiallyEnabled;
        }
    }
    
    public static void WrapWithDisable(object? sender, Action functionBody)
    {
        Code.NotNull(sender, nameof(sender));
        var senderElement = sender as InputElement ??
                            throw new InvalidOperationException(
                                $"Unexpected sender type {sender?.GetType().Name ?? "null"}");

        var initiallyEnabled = senderElement.IsEnabled;
        senderElement.IsEnabled = false;
        
        try
        {
            functionBody();
        }
        finally
        {
            senderElement.IsEnabled = initiallyEnabled;
        }
    }
}