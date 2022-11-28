namespace BuisnessLogicLayer.Models
{
    public class PaginationResultModel<T> where T : class
    {
        public ICollection<T> Data { get; set; } = new List<T>();

        public bool IsDataEmpty {
            get
            {
                return !Data.Any();
            }
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages {
            get {
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            } 
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }


        //TODO: should i use it
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
