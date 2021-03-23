namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int pageNumber, int itemsPerPage, int totalItems, int totalPages)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string Gender { get; set; }
    }
}