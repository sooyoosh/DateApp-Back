namespace DatingApp.Entities
{
    public class Connection
    {
        public string ConnectionId { get; set; }  // SignalR connection ID
        public string Username { get; set; }      // کاربر مربوط به این اتصال
    }
}
