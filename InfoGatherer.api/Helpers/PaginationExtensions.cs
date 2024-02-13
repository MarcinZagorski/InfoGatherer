using InfoGatherer.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace InfoGatherer.api.Helpers
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationPagedResult<T>> GetPage<T, TFilter>(this IQueryable<T> query, PaginationListModel<TFilter> options)
            where T : class
            where TFilter : new()
        {
            var result = new PaginationPagedResult<T>
            {
                CurrentPage = options.Page.Value,
                PageSize = options.ItemsPerPage.Value,
                RowCount = await query.CountAsync()
            };

            if (options.SortBy != null && options.SortBy.Any())
            {
                
                    query = ApplyOrderBy(query, options.SortBy, options.SortDesc);
            }

            var skip = (options.Page.Value - 1) * options.ItemsPerPage.Value;
            query = query.Skip(skip).Take(options.ItemsPerPage.Value);

            result.Results = await query.ToListAsync();

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / options.ItemsPerPage.Value);

            return result;
        }


        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, string[] sortBy, bool[] sortDesc) where T : class
        {
            if (sortBy == null || !sortBy.Any())
            {
                return query; 
            }

            var ordering = string.Join(", ", sortBy.Select((column, index) =>
                $"{column} {(sortDesc.Length > index && sortDesc[index] ? "descending" : "ascending")}"));

            query = query.OrderBy(ordering);

            return query;
        }
    }
}
