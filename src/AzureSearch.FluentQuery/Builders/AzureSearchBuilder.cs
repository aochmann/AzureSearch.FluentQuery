namespace AzureSearch.FluentQuery.Builders;

using System.Linq.Expressions;
using AzureSearch.FluentQuery.Models;
using AzureSearch.FluentQuery.Visitors;
using Constants;

public class AzureSearchBuilder<TModel> : IAzureSearchBuilder<TModel, SearchModel>
    where TModel : class, new()
{
    private readonly SearchModel _searchModel;

    public AzureSearchBuilder()
    {
        _searchModel = new SearchModel();
    }

    public static IAzureSearchBuilder<TModel, SearchModel> Create()
    {
        return new AzureSearchBuilder<TModel>();
    }

    private AzureSearchBuilder(SearchModel searchModel) : this()
    {
        _searchModel.Filters = searchModel.Filters;
        _searchModel.OrderBy = searchModel.OrderBy;
        _searchModel.Select = searchModel.Select;
        _searchModel.SearchFields = searchModel.SearchFields;
    }

    public IAzureSearchBuilder<TModel, SearchModel> Query(Expression<Func<TModel, bool>> queryExp)
    {
        var visitorResult = new AzureSearchVisitor().Build(queryExp.Body);

        var filters = !string.IsNullOrEmpty(_searchModel.Filters)
            ? string.Join(
                " and ",
                new[]
                {
                    _searchModel.Filters,
                    visitorResult
                 }
                 .Where(x => !string.IsNullOrEmpty(x))
                 .Select(x => $"({x})"))
            : visitorResult;

        var newSearchModel = new SearchModel
        {
            Filters = filters,
            OrderBy = _searchModel.OrderBy,
            Select = _searchModel.Select,
            SearchFields = _searchModel.SearchFields
        };

        return new AzureSearchBuilder<TModel>(newSearchModel);
    }

    public IAzureSearchBuilder<TModel, SearchModel> OrderBy(params Expression<Func<TModel, object>>[] ordersExp)
    {
        var orders = ordersExp
            .Select(orderExp => new AzureSearchVisitor().Build(orderExp.Body));

        var resultOrders = (_searchModel.OrderBy ?? Enumerable.Empty<string>())
            .Concat(orders)
            .ToArray();

        var newSearchModel = new SearchModel
        {
            Filters = _searchModel.Filters,
            OrderBy = resultOrders,
            Select = _searchModel.Select,
            SearchFields = _searchModel.SearchFields
        };

        return new AzureSearchBuilder<TModel>(newSearchModel);
    }

    public IAzureSearchBuilder<TModel, SearchModel> SearchFields(params Expression<Func<TModel, object>>[] searchFieldsExp)
    {
        var searchFields = searchFieldsExp
            .Select(searchExp => new AzureSearchVisitor().Build(searchExp.Body));

        var resultSearchFields = (_searchModel.SearchFields ?? Enumerable.Empty<string>())
            .Concat(searchFields)
            .ToArray();

        var newSearchModel = new SearchModel
        {
            SearchFields = resultSearchFields,
            Select = _searchModel.Select,
            Filters = _searchModel.Filters,
            OrderBy = _searchModel.OrderBy
        };

        return new AzureSearchBuilder<TModel>(newSearchModel);
    }

    public IAzureSearchBuilder<TModel, SearchModel> SelectFields(params Expression<Func<TModel, object>>[] selectExp)
    {
        var resultSelect = (_searchModel.SearchFields ?? Enumerable.Empty<string>())
            .Concat(selectExp
                .Select(selectExp => new AzureSearchVisitor().Build(selectExp.Body)))
            .ToArray();

        var newSearchModel = new SearchModel
        {
            SearchFields = _searchModel.SearchFields,
            Select = resultSelect,
            Filters = _searchModel.Filters,
            OrderBy = _searchModel.OrderBy
        };

        return new AzureSearchBuilder<TModel>(newSearchModel);
    }

    public SearchModel Build()
    {
        return new SearchModel
        {
            Filters = _searchModel.Filters,
            OrderBy = _searchModel.OrderBy?.ToArray(),
            SearchFields = _searchModel.SearchFields?.ToArray(),
            Select = _searchModel.Select?.ToArray()
        };
    }
}