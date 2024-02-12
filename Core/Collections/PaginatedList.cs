using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Collections
{
    [JsonConverter(typeof(PaginatedListJsonConverterFactory))]
    public class PaginatedList<T> : IPaginatedList<T>
    {
        public int PageIndex { get; }

        public int PageSize { get; }

        public int Count { get; }

        public IReadOnlyList<T> Items { get; }

        public T this[int index] { get { return Items[index]; } }

        public PaginatedList(int pageIndex, int pageSize, int count, List<T> items)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
