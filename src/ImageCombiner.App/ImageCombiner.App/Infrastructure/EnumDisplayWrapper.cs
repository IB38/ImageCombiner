using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ReactiveUI;

namespace ImageCombiner.App.Infrastructure;

public class EnumDisplayWrapper<TEnum> : ReactiveObject where TEnum : struct, Enum
{
    public string DisplayName
    {
        get => _displayName;
        set => this.RaiseAndSetIfChanged(ref _displayName, value);
    }

    public TEnum Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    private string _displayName;
    private TEnum _value;

    public EnumDisplayWrapper(TEnum en)
    {
        _displayName = GetDisplayName(en);
        _value = en;
    }

    public static List<EnumDisplayWrapper<TEnum>> WrapAllValues()
    {
        var enums = typeof(TEnum).GetEnumValues().Cast<TEnum>();

        return enums.Select(e => new EnumDisplayWrapper<TEnum>(e)).ToList();
    }
    
    private static string GetDisplayName(TEnum en)
    {
        var fi = typeof(TEnum).GetField(en.ToString());
        if (fi is null)
            return en.ToString();

        var displayNameAttr = fi.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr != null)
            return displayNameAttr.DisplayName;
        
        var descriptionAttr = fi.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttr != null)
            return descriptionAttr.Description;

        return en.ToString();
    }
}