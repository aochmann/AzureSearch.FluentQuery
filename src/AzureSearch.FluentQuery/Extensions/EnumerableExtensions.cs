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

    public static bool SearchIsMatch<TSource>(string search)
    {
        return SearchIsMatch<TSource>(
            search,
            Array.Empty<Func<TSource, object>>());
    }

    public static bool SearchIsMatch<TSource>(string search, params Func<TSource, object>[] searchFields)
    {
        return SearchIsMatch<TSource>(search, searchFields, QueryType.Simple);
    }

    public static bool SearchIsMatch<TSource>(
        string search,
        Func<TSource, object>[] searchFields,
        QueryType queryType = QueryType.Simple)
    {
        return SearchIsMatch<TSource>(
            search,
            searchFields,
            queryType,
            SearchMode.Any);
    }

    public static bool SearchIsMatch<TSource>(
        string search,
        Func<TSource, object>[] searchFields,
        QueryType queryType = QueryType.Simple,
        SearchMode searchMode = SearchMode.Any)
    {
        return true;
    }


    public static bool SearchIsMatchScoring<TSource>(this TSource source, params TSource[] searchValues)
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

    public static bool SearchIsMatchScoring<TSource>(string search)
    {
        return SearchIsMatchScoring<TSource>(
            search,
            Array.Empty<Func<TSource, object>>());
    }

    public static bool SearchIsMatchScoring<TSource>(string search, params Func<TSource, object>[] searchFields)
    {
        return SearchIsMatchScoring<TSource>(search, searchFields, QueryType.Simple);
    }

    public static bool SearchIsMatchScoring<TSource>(
        string search,
        Func<TSource, object>[] searchFields,
        QueryType queryType = QueryType.Simple)
    {
        return SearchIsMatchScoring<TSource>(
            search,
            searchFields,
            queryType,
            SearchMode.Any);
    }

    public static bool SearchIsMatchScoring<TSource>(
        string search,
        Func<TSource, object>[] searchFields,
        QueryType queryType = QueryType.Simple,
        SearchMode searchMode = SearchMode.Any)
    {
        return true;
    }
}