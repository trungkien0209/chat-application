using ChatApplication.Hubs;
using ChatApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplication.Services
{
    public class AppService
    {
        DbChat _context;
        public AppService()
        {
            _context = new DbChat();
        }
        public bool Login(LoginData loginData, out int userId)
        {
            userId = 0;
            var currentUser = _context.Users.FirstOrDefault(x => x.Email == loginData.Email && x.Password == loginData.Password);
            if (currentUser != null)
            {
                userId = currentUser.ID;
                return true;
            }
            return false;
        }
        public List<UserDTO> GetUsersToChat()
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            return _context.Users.Where(x=> x.ID != userId).Select(x => new UserDTO
            {
                UserId = x.ID,
                Email = x.Email,
                FullName = x.FullName,
                Photo = x.Photo,
                IsOnline =  true,
            }).ToList();
        }
        internal int AddUserConnection(Guid connectionId)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            _context.UserConnections.Add(new UserConnection
            {
                ConnectionId = connectionId,
                UserId = userId,
            });
            _context.SaveChanges();
            return userId;
        }
        internal int removeUserConnection(Guid connectionId)
        {
            int userId = 0;
            var current = _context.UserConnections.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (current != null)
            {
                userId = current.UserId ?? 0;
                _context.UserConnections.Remove(current);
                _context.SaveChanges();
            }
            return userId;
        }

        internal IList<string> GetUserConnections(int userID)
        {
            return _context.UserConnections.Where(x => x.UserId == userID).Select(x => x.ConnectionId.ToString()).ToList();
        }
        internal IList<string> GetGroupConnections(int groupID)
        {
            return _context.MessageRooms.Where(x => x.ToRoom == groupID).Select(x => x.FromUser.ToString()).ToList();
        }

        internal void RemoveAllUserConnections(int userId)
        {
            var current = _context.UserConnections.Where(x => x.UserId == userId);
            _context.UserConnections.RemoveRange(current);
            _context.SaveChanges();
        }

        internal ChatGroupModel GetChatGroup(int toGroupId)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            var toGroup = _context.Rooms.FirstOrDefault(x => x.RoomID == toGroupId);
            var messages = _context.MessageRooms.Where(x => (x.FromUser == userId && x.ToRoom == toGroupId) || (x.FromUser != userId && x.ToRoom == toGroupId))
                .Select(x => new MessageDTO
                {
                    UserName = x.FromUserName,
                    Message = x.Message,
                    Class = x.FromUser == userId ? "mymess" : "frnmess",
                }).ToList();

            return new ChatGroupModel
            {
                toRoom = ToRoom(toGroup),
                Messages =messages,
            };
        }
        internal ChatBoxModel GetChatBox(int toUserId)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            var toUser = _context.Users.FirstOrDefault(x => x.ID == toUserId);
            var messages = _context.Messages.Where(x => (x.FromUser == userId && x.ToUser == toUserId) || (x.FromUser == toUserId && x.ToUser == userId))
                .Select(x => new MessageDTO { Message = x.Message1,
                Class = x.FromUser == userId ? "mymess" : "frnmess",
                }).ToList();

            return new ChatBoxModel
            {
                ToUser = ToUserDTO(toUser),
                Messages = messages,
                
            };
        }

        public Room ToRoom(Room room)
        {
            return new Room
            {
                RoomID = room.RoomID,
                RoomName = room.RoomName,
            };
        }

        public UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                FullName = user.FullName,
                UserId = user.ID,
                Email = user.Email,
                Photo = user.Photo,
            };
        }

        internal bool SendMessage(int toUserId, string message)
        {
            try
            {
                int USER_ID = int.Parse(HttpContext.Current.User.Identity.Name);
                _context.Messages.Add(new Message
                {
                    FromUser = USER_ID,
                    ToUser = toUserId,
                    Message1 = message,
                    Date = DateTime.Now,
                });
                _context.SaveChanges();
                ChatHub.RecieveMessage(USER_ID, toUserId, message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool SendMessageGroup(int toGroupId, string message)
        {
            try
            {
                int USER_ID = int.Parse(HttpContext.Current.User.Identity.Name);
                var USER_NAME = _context.Users.FirstOrDefault(x => x.ID == USER_ID)?.FullName;
                _context.MessageRooms.Add(new MessageRoom
                {
                    FromUser = USER_ID,
                    ToRoom = toGroupId,
                    Message = message,
                    Date = DateTime.Now,
                    FromUserName = USER_NAME,
                });
                _context.SaveChanges();
                ChatHub.RecieveMessageGroup(USER_ID, toGroupId, message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}