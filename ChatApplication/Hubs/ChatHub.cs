using ChatApplication.Models;
using ChatApplication.Services;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        public static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        public override Task OnConnected()
        {
            int userId = new AppService().AddUserConnection(Guid.Parse(Context.ConnectionId));
            Clients.All.BroadcastOnlineUser(userId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            int userId = new AppService().removeUserConnection(Guid.Parse(Context.ConnectionId));
            Clients.All.BroadcastOfflineUser(userId);
            return base.OnDisconnected(stopCalled);
        }
        public void GetUsersToChat()
        {
            int UserId = int.Parse(HttpContext.Current.User.Identity.Name);
            List<UserDTO> users = new AppService().GetUsersToChat();
            Clients.Clients(new AppService().GetUserConnections(UserId)).BroadcastUsersToChat(users);
        }

        public static void OfflineUser(int UserId)
        {
            context.Clients.All.BroadcastOfflineUser(UserId);
        }

        public static void RecieveMessage(int fromUserId, int toUserId, string message)
        {
            context.Clients.Clients(new AppService().GetUserConnections(toUserId)).BroadcastRecieveMessage(fromUserId, message);
        }

        public static void RecieveMessageGroup(int fromUserId, int toGroupId, string message)
        {
            context.Clients.Clients(new AppService().GetGroupConnections(toGroupId)).BroadcastRecieveMessage(fromUserId, message);
        }

        public async Task AddUserToGroup(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).addChatMessage(Context.User.Identity.Name + " joined.");
        }
    }
}