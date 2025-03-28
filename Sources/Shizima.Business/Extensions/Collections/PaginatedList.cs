namespace Shizima.Business.Extensions.Collections
{
    /// <summary>
    /// A collection that manage pagination mechanism.
    /// </summary>
    /// <typeparam name="T">The type of inner element.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// The zero base index of first elements on pagination.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// The zero base index of last elements.
        /// </summary>
        public int EndIndex { get; }

        /// <summary>
        /// Total elements that ca be retrieve.
        /// </summary>
        public int TotalRecords { get; }

        /// <summary>
        /// Initialize a <see cref="PaginatedList{T}"/> with items.
        /// </summary>
        /// <param name="items">elements</param>
        /// <param name="startIndex">start index of element retrieved</param>
        /// <param name="endIndex">end index of elements retrieved</param>
        /// <param name="totalRecords">total of elements that can be retrieved</param>
        public PaginatedList(IEnumerable<T> items, int startIndex, int endIndex, int totalRecords)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            TotalRecords = totalRecords;

            if (items != null) AddRange(items);
        }

        /// <summary>
        /// Initialize a <see cref="PaginatedList{T}"/> with items
        /// </summary>
        /// <param name="items"></param>
        public PaginatedList(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return;

            AddRange(items);
            StartIndex = 0;
            EndIndex = Count - 1;
            TotalRecords = Count;
        }

        /// <summary>
        /// Indicate if a previous page can be retrieved
        /// </summary>
        public bool HasPreviousPage => StartIndex > 0;

        /// <summary>
        /// Indicate if a next page can be retrieved
        /// </summary>
        public bool HasNextPage => EndIndex < TotalRecords - 1;

        /// <summary>
        /// Generate an empty paginated list
        /// </summary>
        /// <returns></returns>
        public static PaginatedList<T> Empty()
        {
            return new PaginatedList<T>(Enumerable.Empty<T>());
        }
    }

    /// <summary>
    /// PaginatedList extensions.
    /// </summary>
    public static class PaginatedListExtension
    {
        /// <summary>
        /// Allow to map this paginated list of <typeparamref name="T"/> to a paginated list of <typeparamref name="TMap"/>
        /// </summary>
        /// <typeparam name="T">The type of source element in <paramref name="this"/></typeparam>
        /// <typeparam name="TMap">The type to convert on.</typeparam>
        /// <param name="this">The source paginated list to convert.</param>
        /// <param name="converter">The function to convert.</param>
        /// <returns>A <see cref="PaginatedList{TMap}"/></returns>
        public static PaginatedList<TMap> MapTo<T, TMap>(this PaginatedList<T> @this, Func<T, TMap> converter)
        {
            return new PaginatedList<TMap>(@this.Select(converter), @this.StartIndex, @this.EndIndex, @this.TotalRecords);
        }
    }

    /// <summary>
    /// Provide extensions for collections.
    /// </summary>
    public static class CustomCollectionExtensions
    {
        /// <summary>
        /// Create a <see cref="PaginatedList{T}"/> of an ordered query.
        /// </summary>
        /// <typeparam name="T">The object type that will be contained in the paginated list</typeparam>
        /// <param name="source">The query source to convert.</param>
        /// <param name="pageIndex">The page index to query.</param>
        /// <param name="pageSize">The number of element per page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task of <see cref="PaginatedList{T}"/></returns>
        public static PaginatedList<T> ToPaginatedListAsync<T>(this ICollection<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var count = source.Count();
            var items = source.Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int start = Math.Min(pageIndex * pageSize + 1, count);
            int end = Math.Min((pageIndex + 1) * pageSize, count);

            return new PaginatedList<T>(items, start - 1, end - 1, count);
        }
    }
}
