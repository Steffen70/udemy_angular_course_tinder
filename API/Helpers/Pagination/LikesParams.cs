namespace API.Helpers.Pagination
{
    public class LikesParams : PaginationParams
    {
        public string Predicate { get; set; } 
        public int UserId { get; set; }
    }
}