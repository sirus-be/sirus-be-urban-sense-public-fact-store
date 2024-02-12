using System.Collections;
using System.Collections.Generic;

namespace Core.Collections
{
    public interface IPaginatedList<out T> : IPaginatedList, IEnumerable<T>, IEnumerable
    {
        IReadOnlyList<T> Items { get; }
        T this[int index] { get; }
    }

    public interface IPaginatedList
    {
        int PageIndex { get; }
        int PageSize { get; }
        int Count { get; }
    }
}
