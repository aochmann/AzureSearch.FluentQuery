using System;
using System.Linq;
using AzureSearch.FluentQuery.Builders;
using AzureSearch.FluentQuery.Constants;
using AzureSearch.FluentQuery.Extensions;
using AzureSearch.FluentQuery.Models;
using AzureSearch.FluentQuery.UnitTests.Models;
using Shouldly;
using Xunit;

namespace AzureSearch.FluentQuery.UnitTests.Builders;

public class AzureSearchBuilderTests : IClassFixture<AzureSearchBuilder<ExampleTestModel>>
{
    private readonly IAzureSearchBuilder<ExampleTestModel, SearchModel> _azureSearchBuilder;

    public AzureSearchBuilderTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder)
    {
        _azureSearchBuilder = azureSearchBuilder;
    }

    public class StringTests : AzureSearchBuilderTests
    {
        public StringTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_Return_EqualsQuery_When_EqualsCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Name.Equals("Example Name")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.Equal} 'Example Name'"));
        }

        [Fact]
        public void Should_Return_EqualsQuery_When_OperatorCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Name == "Example Name").Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.Equal} 'Example Name'"));
        }

        [Fact]
        public void Should_Return_NotEqualsQuery_When_OperatorCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Name != "Example Name").Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.NotEqual} 'Example Name'"));
        }

        [Fact]
        public void Should_Return_EqualsQuery_When_NullComparison()
        {
            var result = _azureSearchBuilder.Query(model => model.Name == null).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.Equal} {AzureSearchSyntax.Null}"));
        }

        [Fact]
        public void Should_Return_NotEqualsQuery_When_NullComparison()
        {
            var result = _azureSearchBuilder.Query(model => model.Name != null).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.NotEqual} {AzureSearchSyntax.Null}"));
        }

        [Fact]
        public void Should_Return_Query_With_WrappedNotOperator()
        {
            var result = _azureSearchBuilder.Query(model => !(model.Name == null)).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{AzureSearchSyntax.Not} ({nameof(ExampleTestModel.Name)} {AzureSearchSyntax.Equal} {AzureSearchSyntax.Null})"));
        }
    }

    public class IntTests : AzureSearchBuilderTests
    {
        public IntTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_Return_EqualsQuery_When_EqualsCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Number.Equals(2)).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Number)} {AzureSearchSyntax.Equal} 2"));
        }

        [Fact]
        public void Should_Return_EqualsQuery_When_OperatorCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Number == 2).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Number)} {AzureSearchSyntax.Equal} 2"));
        }

        [Fact]
        public void Should_Return_NotEqualsQuery_When_OperatorCall()
        {
            var result = _azureSearchBuilder.Query(model => model.Number != 2).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Number)} {AzureSearchSyntax.NotEqual} 2"));
        }
    }

    public class LinqTests : AzureSearchBuilderTests
    {
        public LinqTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_Return_AnyQuery()
        {
            var result = _azureSearchBuilder.Query(model => model.Tags.Any(x => x != "Example Tag")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.Any}(x {Separators.ArrowFunction} x {AzureSearchSyntax.NotEqual} 'Example Tag')"));
        }

        [Fact]
        public void Should_Return_AnyQuery_With_WrappedNotOperator()
        {
            var result = _azureSearchBuilder.Query(model => !model.Tags.Any(x => x != "Example Tag")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{AzureSearchSyntax.Not} ({nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.Any}(x {Separators.ArrowFunction} x {AzureSearchSyntax.NotEqual} 'Example Tag'))"));
        }

        [Fact]
        public void Should_Return_AllQuery()
        {
            var result = _azureSearchBuilder.Query(model => model.Tags.All(x => x != "Example Tag")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.All}(x {Separators.ArrowFunction} x {AzureSearchSyntax.NotEqual} 'Example Tag')"));
        }

        [Fact]
        public void Should_Return_AllQuery_With_WrappedNotOperator()
        {
            var result = _azureSearchBuilder.Query(model => !model.Tags.All(x => x != "Example Tag")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{AzureSearchSyntax.Not} ({nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.All}(x {Separators.ArrowFunction} x {AzureSearchSyntax.NotEqual} 'Example Tag'))"));
        }
    }

    public class RuntimeCallTests : AzureSearchBuilderTests
    {
        public RuntimeCallTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_GetValue_FromLocalVariable()
        {
            const string localVariable = "local_variable";
            var result = _azureSearchBuilder.Query(model => model.Name == localVariable).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.Name)} {AzureSearchSyntax.Equal} '{localVariable}'"));
        }

        [Fact]
        public void Should_Call_DateTimeNow()
        {
            var now = DateTime.UtcNow;
            var result = _azureSearchBuilder.Query(model => model.PublishedAt >= now).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldBe($"{nameof(ExampleTestModel.PublishedAt)} {AzureSearchSyntax.GreaterThanOrEqual} {now:O}"));
        }

        [Fact]
        public void Should_Call_DateTimeNowWitSubFunction()
        {
            var now = DateTimeOffset.UtcNow;
            var futureDateTime = $"{now.AddDays(2).AddDays(2):O}";

            var result = _azureSearchBuilder.Query(model => model.PublishedAt >= DateTimeOffset.UtcNow.AddDays(2).AddDays(2)).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldStartWith($"{nameof(ExampleTestModel.PublishedAt)} {AzureSearchSyntax.GreaterThanOrEqual} {futureDateTime[0..16]}"));
        }
    }

    public class NestedTests : AzureSearchBuilderTests
    {
        public NestedTests(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_Return_NestedAnyQuery()
        {
            var result = _azureSearchBuilder.Query(model => model.NestedItems.Any(nI => nI.Name == "Name")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldStartWith($"{nameof(ExampleTestModel.NestedItems)}/{AzureSearchSyntax.Any}(nI {Separators.ArrowFunction} {nameof(SubExampleItemTest.Name)} {AzureSearchSyntax.Equal} 'Name')"));
        }

        [Fact]
        public void Should_Return_NestedAllQuery()
        {
            var result = _azureSearchBuilder.Query(model => model.NestedItems.All(nI => nI.Name == "Name")).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldStartWith($"{nameof(ExampleTestModel.NestedItems)}/{AzureSearchSyntax.All}(nI {Separators.ArrowFunction} {nameof(SubExampleItemTest.Name)} {AzureSearchSyntax.Equal} 'Name')"));
        }
    }

    public class AzureSearchFunctions : AzureSearchBuilderTests
    {
        public AzureSearchFunctions(AzureSearchBuilder<ExampleTestModel> azureSearchBuilder) : base(azureSearchBuilder)
        {
        }

        [Fact]
        public void Should_Return_SearchInQuery()
        {
            var result = _azureSearchBuilder.Query(model => model.Tags.Any(tag => EnumerableExtensions.SearchIn(tag, new[] { "tag1", "tag2" }))).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldStartWith($"{nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.Any}(tag {Separators.ArrowFunction} {AzureSearchSyntax.SearchIn}(tag, 'tag1, tag2'))"));
        }

        [Fact]
        public void Should_SearchIn_With_ExtensionMethod()
        {
            var result = _azureSearchBuilder.Query(model => model.Tags.Any(tag => tag.SearchIn(new[] { "tag1", "tag2" }))).Build();
            result.ShouldSatisfyAllConditions(nameof(AzureSearchBuilderTests),
                () => result.Filters.ShouldNotBeNull(),
                () => result.Filters.ShouldStartWith($"{nameof(ExampleTestModel.Tags)}/{AzureSearchSyntax.Any}(tag {Separators.ArrowFunction} {AzureSearchSyntax.SearchIn}(tag, 'tag1, tag2'))"));
        }
    }
}