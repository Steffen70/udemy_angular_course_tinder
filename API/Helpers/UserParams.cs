using System.ComponentModel.DataAnnotations;

namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string CurrentUsername { get; set; }
        [RegularExpression("(male)|(female)", ErrorMessage = "The Gender must be either 'male' or 'female'")]
        public string Gender { get; set; }
        [Range(18, int.MaxValue, ErrorMessage = "No members are younger than 18")]
        public int MinAge { get; set; } = 18;
        private int? _maxAge;
        public int? MaxAge
        {
            get => _maxAge;
            set => _maxAge = value < MinAge ? MinAge : value;
        }
        [RegularExpression("(created)|(lastActive)", ErrorMessage = "You can order by 'created' or 'lastActive'")]
        public string OrderBy { get; set; } = "lastActive";
    }
}