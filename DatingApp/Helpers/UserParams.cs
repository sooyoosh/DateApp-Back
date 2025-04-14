namespace DatingApp.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50; // حداکثر تعداد آیتم‌ها در هر صفحه
        public int PageNumber { get; set; } = 1; // شماره صفحه پیش‌فرض

        private int _pageSize = 5; // مقدار پیش‌فرض تعداد آیتم‌ها در هر صفحه
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; // محدودیت حداکثر سایز صفحه
        }
        public string? Gender { get; set; }
        public string? CurrentUserName { get; set; }
        public int MinAge { get; set; } = 18; // حداقل سن کاربران
        public int MaxAge { get; set; } = 100; // حداکثر سن کاربران


        public string? OrderBy { get; set; } = "lastActive";

    }
}
