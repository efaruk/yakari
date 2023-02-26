namespace Yakari;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Argument validation helpers.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArgumentValidationHelperExtensions
{
    public static string NotNullOrWhitespace(this string arg, string argName)
    {
        if (arg == null)
        {
            throw new ArgumentNullException(argName);
        }

        if (string.IsNullOrWhiteSpace(arg))
        {
            throw new ArgumentException($"{argName} can not be empty or whitespace.", argName);
        }

        return arg;
    }

    public static T NotNull<T>(this T arg, string argName)
        where T : class
    {
        return arg ?? throw new ArgumentNullException(argName);
    }

    public static ICollection<T> NotNullOrEmpty<T>(this ICollection<T> arg, string argName)
    {
        _ = arg.NotNull(argName);

        if (arg.Count == 0)
        {
            throw new ArgumentException($"{argName} collection can not be empty.", argName);
        }

        return arg;
    }

    public static IList<T> NotNullOrEmpty<T>(this IList<T> arg, string argName)
    {
        _ = arg.NotNull(argName);

        if (arg.Count == 0)
        {
            throw new ArgumentException($"{argName} list can not be empty.", argName);
        }

        return arg;
    }

    public static IEnumerable<T> NotNullOrEmpty<T>(this IEnumerable<T> arg, string argName)
    {
        _ = arg.NotNull(argName);

        if (!arg.Any())
        {
            throw new ArgumentException($"{argName} enumerable can not be empty.", argName);
        }

        return arg;
    }

    public static T InRange<T>(this T arg, T min, T max, string argName)
        where T : IComparable
    {
        if (arg.CompareTo(min) < 0)
        {
            throw new ArgumentOutOfRangeException(
                argName,
                arg,
                $"{argName} should be between '{min}' and '{max}'.");
        }

        return arg;
    }

    public static T NotDefault<T>(this T arg, string argName)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(arg, default))
        {
            throw new ArgumentException($"{argName} has default value.", argName);
        }

        return arg;
    }
}