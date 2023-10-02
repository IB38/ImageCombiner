using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageCombiner.App.Infrastructure;

public class CollectionNotEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType.IsAssignableTo(typeof(bool)))
        {
            switch (value)
            {
                case null:
                    return false;
                case ICollection collection:
                    return collection.Count > 0;
            }
        }
        
        return new BindingNotification(
            new InvalidCastException($"{nameof(CollectionNotEmptyConverter)} can't convert {value?.GetType().Name} to {targetType.Name}"), 
            BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Only ToSource binding is supported");
    }
}