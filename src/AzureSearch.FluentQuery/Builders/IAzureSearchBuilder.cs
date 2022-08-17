using System.Linq.Expressions;
using AzureSearch.FluentQuery.Models;

namespace AzureSearch.FluentQuery.Builders;

public interface IAzureSearchBuilder<TModel, TSearchModel> where TModel : class, new()
{
    IAzureSearchBuilder<TModel, SearchModel> SearchFields(params Expression<Func<TModel, object>>[] searchFieldsExp);
    IAzureSearchBuilder<TModel, SearchModel> SelectFields(params Expression<Func<TModel, object>>[] selectExp);
    IAzureSearchBuilder<TModel, SearchModel> Query(Expression<Func<TModel, bool>> queryExp);
    IAzureSearchBuilder<TModel, TSearchModel> OrderBy(params Expression<Func<TModel, object>>[] ordersExp);
    TSearchModel Build();
}
