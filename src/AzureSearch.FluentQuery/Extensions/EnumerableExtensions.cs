using AzureSearch.FluentQuery.Enums;

namespace AzureSearch.FluentQuery.Extensions;

public static class EnumerableExtensions
{
    public static bool SearchIn<TSource>(TSource source, IEnumerable<TSource> searchValues)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (searchValues == null)
        {
            throw new ArgumentNullException(nameof(searchValues));
        }

        return searchValues.Any(x => x?.Equals(source) ?? false);
    }

    public static bool SearchIn<TSource>(this TSource source, params TSource[] searchValues)
    {
        if (source == null)
        {
            return false;
        }

        if (searchValues == null)
        {
            throw new ArgumentNullException(nameof(searchValues));
        }

        return searchValues.Any(x => x?.Equals(source) ?? false);
    }
}