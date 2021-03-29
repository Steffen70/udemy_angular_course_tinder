using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.GetUserName();

            await _tracker.UserConnected(username, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", username);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.GetUserName();

            await _tracker.UserDisconnected(username, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", username);

            await base.OnDisconnectedAsync(exception);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }
    }
}