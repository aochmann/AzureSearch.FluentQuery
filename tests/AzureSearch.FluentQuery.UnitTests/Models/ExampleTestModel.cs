using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.FluentQuery.UnitTests.Models;

public class ExampleTestModel
{
    public string Name { get; set; } = null!;
    public int Number { get; set; }
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<SubExampleItemTest> NestedItems { get; set; } = Enumerable.Empty<SubExampleItemTest>();
    public DateTime PublishedAt { get; set; }
}

public class SubExampleItemTest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
