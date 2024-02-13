namespace InfoGatherer.api.Models
{
    public class PaginationListModel<TFilter> where TFilter : new()
    {
        public TFilter Filter { get; set; } = new TFilter();
        public int? ItemsPerPage { get; set; } = 10;
        public int? Page { get; set; } = 1;
        public string[] SortBy { get; set; }
        public bool[] SortDesc { get; set; }
    }
}
