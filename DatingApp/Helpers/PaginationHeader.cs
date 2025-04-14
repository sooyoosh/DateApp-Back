namespace DatingApp.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; } // شماره صفحه فعلی
        public int ItemsPerPage { get; set; } // تعداد آیتم‌ها در هر صفحه
        public int TotalItems { get; set; } // تعداد کل آیتم‌ها
        public int TotalPages { get; set; } // تعداد کل صفحات

        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
