﻿using System.ComponentModel.DataAnnotations;

namespace CodingTracker.utils;

/// <summary>
/// The Utilities class provides common utility methods.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Gets the enum values and their display names.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <returns>
    /// A collection of key-value pairs where the key is the enum value
    /// and the value is the display name associated with the value.
    /// </returns>
    internal static IEnumerable<KeyValuePair<TEnum, string>>
        GetEnumValuesAndDisplayNames<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(enumValue => new KeyValuePair<TEnum, string>(
                enumValue,
                enumValue.GetType()
                    .GetField(enumValue.ToString())
                    ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .Cast<DisplayAttribute>()
                    .FirstOrDefault()?.Name ?? enumValue.ToString()
            ));
    }
}