using System.ComponentModel.DataAnnotations;

namespace CodingTracker.utils;

/// <summary>
/// The Utilities class provides common utility methods.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Retrieves the display name of the specified enum value.
    /// </summary>
    /// <param name="enumValue">The enum to process.</param>
    /// <returns>The display name of the enum value if it has a DisplayAttribute, otherwise the string representation of the enum value.</returns>
    private static string? GetDisplayName(this Enum enumValue)
    { 
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).SingleOrDefault();
        var attributes = memberInfo?.GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attributes is not { Length: > 0 })
        {
            return enumValue.ToString();
        }
        
        var displayAttribute = (DisplayAttribute)attributes[0];
            
        return displayAttribute.Name;

    }

    /// <summary>
    /// Retrieves the menu entries for the given enum value.
    /// </summary>
    /// <param name="enumValue">The enum to process.</param>
    /// <returns>An array of strings representing the menu entries.</returns>
    internal static string?[]? GetMenuEntries(Type enumValue)
    {
        if (!enumValue.IsEnum)
        {
            return null;
        }

        var enumValues = Enum.GetValues(enumValue);
        var menuEntries = new string?[enumValues.Length];

        for (var i = 0; i < enumValues.Length; i++)
        {
            menuEntries[i] = ((Enum)enumValues.GetValue(i)).GetDisplayName();
        }

        return menuEntries;
    }
}