using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AracKiralamaPortal.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserConnectionInfo> _onlineUsers
            = new ConcurrentDictionary<string, UserConnectionInfo>();

        public class UserConnectionInfo
        {
            public string Username { get; set; }
            public List<string> ConnectionIds { get; set; } = new List<string>();
        }

        // ---------------- REGISTER ----------------
        public async Task RegisterUser(string userId, string username)
        {
            var user = _onlineUsers.GetOrAdd(userId, _ => new UserConnectionInfo());
            user.Username = username;

            lock (user.ConnectionIds)
            {
                if (!user.ConnectionIds.Contains(Context.ConnectionId))
                    user.ConnectionIds.Add(Context.ConnectionId);
            }

            await BroadcastOnlineUsers();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var user in _onlineUsers)
            {
                lock (user.Value.ConnectionIds)
                {
                    if (user.Value.ConnectionIds.Remove(Context.ConnectionId) &&
                        user.Value.ConnectionIds.Count == 0)
                    {
                        _onlineUsers.TryRemove(user.Key, out _);
                    }
                }
            }

            await BroadcastOnlineUsers();
            await base.OnDisconnectedAsync(exception);
        }

        // ---------------- MESSAGES ----------------
        public async Task SendMessage(string message)
        {
            var sender = GetSender();
            if (sender.UserId == null) return;

            await Clients.All.SendAsync("ReceiveMessage", new
            {
                Sender = sender.UserId,
                SenderName = sender.Username,
                Text = message,
                Time = DateTime.Now.ToString("HH:mm:ss")
            });
        }

        public async Task SendPrivateMessage(string receiverUserId, string message)
        {
            var sender = GetSender();
            if (sender.UserId == null) return;

            string receiverName = "User";

            if (_onlineUsers.TryGetValue(receiverUserId, out var receiver))
            {
                receiverName = receiver.Username;

                foreach (var conn in receiver.ConnectionIds)
                {
                    await Clients.Client(conn).SendAsync("ReceivePrivateMessage", new
                    {
                        Sender = sender.UserId,
                        SenderName = sender.Username,
                        Receiver = receiverUserId,
                        ReceiverName = receiverName,
                        Text = message,
                        Time = DateTime.Now.ToString("HH:mm:ss")
                    });
                }
            }

            await Clients.Caller.SendAsync("ReceivePrivateMessage", new
            {
                Sender = sender.UserId,
                SenderName = sender.Username,
                Receiver = receiverUserId,
                ReceiverName = receiverName,
                Text = message,
                Time = DateTime.Now.ToString("HH:mm:ss")
            });
        }

        // ---------------- TYPING ----------------
        public async Task Typing(string targetUserId)
        {
            var sender = GetSender();
            if (sender.UserId == null) return;

            if (!string.IsNullOrEmpty(targetUserId) &&
                _onlineUsers.TryGetValue(targetUserId, out var receiver))
            {
                foreach (var conn in receiver.ConnectionIds)
                    await Clients.Client(conn).SendAsync("UserTyping", sender.UserId);
            }
            else
            {
                await Clients.Others.SendAsync("UserTyping", sender.UserId);
            }
        }

        public async Task StopTyping(string targetUserId)
        {
            var sender = GetSender();
            if (sender.UserId == null) return;

            if (!string.IsNullOrEmpty(targetUserId) &&
                _onlineUsers.TryGetValue(targetUserId, out var receiver))
            {
                foreach (var conn in receiver.ConnectionIds)
                    await Clients.Client(conn).SendAsync("UserStoppedTyping", sender.UserId);
            }
            else
            {
                await Clients.Others.SendAsync("UserStoppedTyping", sender.UserId);
            }
        }

        // ---------------- HELPERS ----------------
        private async Task BroadcastOnlineUsers()
        {
            var users = _onlineUsers.Select(x => new
            {
                id = x.Key,
                username = x.Value.Username
            });

            await Clients.All.SendAsync("UpdateOnlineUsers", users);
        }

        private (string UserId, string Username) GetSender()
        {
            var sender = _onlineUsers.FirstOrDefault(x =>
                x.Value.ConnectionIds.Contains(Context.ConnectionId));

            if (string.IsNullOrEmpty(sender.Key))
                return (null, null);

            return (sender.Key, sender.Value.Username);
        }
    }
}
