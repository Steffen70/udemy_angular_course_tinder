namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int CurrentPage { get; set; } = 1;

        private int _itemsPerPage = 10;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set => _itemsPerPage = value > MaxPageSize ? MaxPageSize : value;
        }

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
    }
}