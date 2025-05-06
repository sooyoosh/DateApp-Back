using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DatingApp.Helpers
{
    public class MessageParams: PaginationParams
    {
        
        public string? Username { get; set; } 

        // فیلتر پیام‌ها: inbox, outbox, unread
        public string Container { get; set; } = "Unread";
    }
}
