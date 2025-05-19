using CloudinaryDotNet.Actions;

namespace DatingApp.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = [];

        public Task UserConnected(string username, string connectionId) {

            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers[username] = new List<string> { connectionId };
                  
                }
                return Task.CompletedTask;
            }

       
        }

        public Task UserDisconnected(string username, string connectionId)
        {


            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;


                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);

                }
                return Task.CompletedTask;
            }




        }

        public Task<List<string>> GetOnlineUsers()
        {
            List<string> onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.Keys.OrderBy(x => x).ToList();
            }

            return Task.FromResult(onlineUsers);
        }

    }
}
