namespace AzureSearch.FluentQuery.Models;

public class SearchModel
{
    public IEnumerable<string>? Select { get; set; }
    public IEnumerable<string>? SearchFields { get; set; }
    public string? Filters { get; set; }
    public IEnumerable<string>? OrderBy { get; set; }
}
