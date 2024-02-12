using System;
using System.Linq;

namespace Core.Collections
{
    public static class IPaginatedListExtensions
    {
        public static int PageCount(this IPaginatedList source)
        {
            return (int)Math.Ceiling((double)source.Count / source.PageSize);
        }

        public static bool HasPreviousPage(this IPaginatedList source)
        {
            return (source.PageIndex > 0);
        }

        public static bool HasNextPage(this IPaginatedList source)
        {
            return (source.PageIndex < (source.PageCount() - 1));
        }

        public static IPaginatedList<TResult> Select<TSource, TResult>(this IPaginatedList<TSource> source, Func<TSource, TResult> selector)
        {
            return new PaginatedList<TResult>(source.PageIndex, source.PageSize, source.Count, source.Items.Select(selector).ToList());
        }
    }
}
