namespace API.Helpers
{
    public class ApplicationSettings
    {
        public string[] Roles { get; set; }
        public string AdminPassword { get; set; }
        public string[] AdminRoles { get; set; }
        public string MemberRole { get; set; }
    }
}