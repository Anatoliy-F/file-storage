namespace BuisnessLogicLayer.Models
{
    public class QueryModel
    {
        /// <summary>
        /// Requested page number
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// Requested page size
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Column by which to sort
        /// </summary>
        public string? SortColumn { get; set; } = null;

        /// <summary>
        /// Sorting direction ["ASC" | "DESC"]
        /// </summary>
        public string? SortOrder { get; set; } = null;

        /// <summary>
        /// Column by which values will be filtered
        /// </summary>
        public string? FilterColumn { get; set; } = null;

        /// <summary>
        /// Filter query
        /// </summary>
        public string? FilterQuery { get; set; } = null;
    }
}
