namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// Represents a request response object with pagination, sorting, and filtering
    /// </summary>
    /// <typeparam name="T">Represent reference value</typeparam>
    public class PaginationResultModel<T> where T : class
    {
        /// <summary>
        /// Collection of return values
        /// </summary>
        public ICollection<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Return TRUE if Data properties empty collection
        /// </summary>
        public bool IsDataEmpty {
            get
            {
                return !Data.Any();
            }
        }

        /// <summary>
        /// Current page number
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of available objects
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total pages available
        /// </summary>
        public int TotalPages {
            get {
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            } 
        }

        /// <summary>
        /// Return TRUE if current page isn't first
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        /// <summary>
        /// Return TRUE if current page isn't last
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }

        /// <summary>
        /// Sorting Column name (or null if none set)
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Sorting Order ("ASC", "DESC" or null if none set)
        /// </summary>
        public string? SortOrder { get; set; }

        /// <summary>
        /// Filter Column name (or null if none set)
        /// </summary>
        public string? FilterColumn { get; set; }

        /// <summary>
        /// Filter Query string 
        /// (to be used within the given FilterColumn)
        /// </summary>
        public string? FilterQuery { get; set; }
    }
}
