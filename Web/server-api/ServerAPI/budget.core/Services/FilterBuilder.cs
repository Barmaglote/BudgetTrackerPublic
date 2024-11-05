using budget.core.Models;
using budget.core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace budget.core.Services {
  public class FilterBuilder<T> : IFilterBuilder<T> where T : class {

    public FilterDefinition<T>? BuildFilerDefinition(TableLazyLoadEvent tableLazyLoadEvent) {
      if (tableLazyLoadEvent == null || tableLazyLoadEvent.Filters == null || !tableLazyLoadEvent.Filters.Any())
      return null;

      FilterDefinition<T>? combinedFilter = null;

      foreach (var filter in tableLazyLoadEvent.Filters) {
        if (filter.Value == null || !filter.Value.Any()) continue;
        FilterDefinition<T>? definition = null;
        var definitions = new List<FilterDefinition<T>>();

        foreach (var item in filter.Value) {
          if (item.Value == null && item.MatchMode?.ToLower() != "neq") continue;

          switch (item.MatchMode?.ToLower()) {
            case "contains":
              definition = Builders<T>.Filter.Regex(filter.Key, new BsonRegularExpression(item.Value?.ToString(), "i"));
              break;

            case "gt":
              definition = Builders<T>.Filter.Gt(filter.Key, item.Value);
              break;

            case "gte":
              if (item.Value == null) break;
              if (DateTime.TryParse(item.Value.ToString(), out DateTime gtedate)) {
                definition = Builders<T>.Filter.Gte(filter.Key, gtedate);
              }

              break;

            case "lt":
              definition = Builders<T>.Filter.Lt(filter.Key, item.Value);
              break;

            case "lte":
              if (item.Value == null) break;
              if (DateTime.TryParse(item.Value.ToString(), out DateTime ltedate)) {
                definition = Builders<T>.Filter.Lte(filter.Key, ltedate);
              }
              break;

            case "eq":
              definition = Builders<T>.Filter.Eq(filter.Key, item.Value?.ToString());
              break;

            case "eqbool":
              definition = Builders<T>.Filter.Eq(filter.Key, item.Value);
              break;

            case "neq":
              definition = (item.Value == null) ? Builders<T>.Filter.Ne(filter.Key, BsonNull.Value) : Builders<T>.Filter.Ne(filter.Key, item.Value);
              break;
          }

          if (definition != null) definitions.Add(definition);
        }

        var operatorMode = filter.Value?.First().OperatorMode;
        if (!string.IsNullOrEmpty(operatorMode)) {
          switch (operatorMode.ToLower()) {
            case "and":
              definition = Builders<T>.Filter.And(definitions);
              break;

            case "or":
              definition = Builders<T>.Filter.Or(definitions);
              break;
          }
        }

        if (combinedFilter == null) {
          combinedFilter = definition;
        } else {
          combinedFilter &= definition;
        }
      }

      return combinedFilter;
    }
  }
}
