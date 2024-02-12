using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Collections
{
    public static class IQueryableExtensions
    {
        public static IPaginatedList<T> ToPaginatedList<T>(this IQueryable<T> collection, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            var totalItems = collection.Count();
            if (pageSize > 0)
            {
                var items = collection.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                return new PaginatedList<T>(pageIndex, pageSize, totalItems, items);
            }
            else
            {
                var items = collection.ToList();
                return new PaginatedList<T>(0, totalItems, totalItems, items);
            }
        }
    }
}
