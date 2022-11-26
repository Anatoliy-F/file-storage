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
    }
}
