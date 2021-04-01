namespace API.Helpers
{
    public class RoleConfiguration
    {
        public string[] Roles { get; set; } = { "Member", "Moderator", "Admin" };
        public string[] AdminRoles { get; set; } = { "Moderator", "Admin" };
        public string MemberRole { get; set; } = "Member";
    }
}