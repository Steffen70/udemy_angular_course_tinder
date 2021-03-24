namespace API.Helpers
{
    public abstract class PaginationHeader
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public void Populate<TList>(PagedList<TList> pagedList)
        {
            this.PageNumber = pagedList.PageNumber;
            this.ItemsPerPage = pagedList.PageSize;
            this.TotalItems = pagedList.TotalCount;
            this.TotalPages = pagedList.TotalPages;
        }
    }
}