namespace DatingApp.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUserName { get; set; } // شناسه گیرنده
        public string Content { get; set; }  // محتوای پیام
    }
}
