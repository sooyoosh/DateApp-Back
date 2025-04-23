namespace DatingApp.Helpers
{
    public class UserParams:PaginationParams
    {
        
        public string? Gender { get; set; }
        public string? CurrentUserName { get; set; }
        public int MinAge { get; set; } = 18; // حداقل سن کاربران
        public int MaxAge { get; set; } = 100; // حداکثر سن کاربران


        public string? OrderBy { get; set; } = "lastActive";

    }
}
