using DatingApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    public class PresenceHub(PresenceTracker presenceTracker):Hub
    {

        [Authorize]

        public override async Task OnConnectedAsync()
        {
            if (Context.User == null) throw new HubException("cannot get current user claim");

            await presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline",Context.User?.GetUsername());
            var userOnline = presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUser", userOnline);


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User == null) throw new HubException("cannot get current user claim");

            await presenceTracker.UserDisconnected(Context.User.GetUsername(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());
            var userOnline = presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUser", userOnline);

            await base.OnDisconnectedAsync(exception);

        }






    }
}
