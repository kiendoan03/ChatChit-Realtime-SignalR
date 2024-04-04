using AutoMapper;
using ChatChit.DAL;
using ChatChit.Models;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ChatChit.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        private readonly IMapper _mapper;
        public readonly static List<UserViewModel> _Connections = new List<UserViewModel>();
        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();


        public ChatHub(ChatDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessageConnect", "System", $"{Context.ConnectionId} joined the chat");
            await base.OnConnectedAsync();
        }

        public  async Task GetUserActive()
        {
            var user = await _context.Users.FindAsync(Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveUserActive", user);
        }

        public async Task GetChatHistoryLobby()
        {
            //var messages = await _context.Messages.Where(m => m.RoomId == null && m.ToUserId == null).Include(m => m.FromUser).ToListAsync();
            //var messagesViewModel = _mapper.Map<List<Message>, List<MessageViewModel>>(messages);
            //await Clients.Caller.SendAsync("ReceiveChatHistoryLobby", messagesViewModel);
            var messages = await _context.Messages.Where(m => m.RoomId == null && m.ToUserId == null).Include(m => m.FromUser).ToListAsync();
            var messagesViewModel = _mapper.Map<List<Message>, List<MessageViewModel>>(messages);
            await Clients.Caller.SendAsync("ReceiveChatHistoryLobby", messagesViewModel);
        }

        public async Task SendMessage(string userId, string message)
        {
            var user = await _context.Users.FindAsync(userId);
            var mgs = new Message
            {
                FromUserId = userId,
                Content = Regex.Replace(message, @"<.*?>", string.Empty),
                SendAt = DateTime.Now
            };
            _context.Messages.Add(mgs);
            await _context.SaveChangesAsync();
            var messageViewModel = _mapper.Map<Message, MessageViewModel>(mgs);
            //await Clients.All.SendAsync("ReceiveMessage", user.DisplayName, message);
            await Clients.All.SendAsync("ReceiveMessage", messageViewModel);
        }

        public async Task SendPrivate(string fromUserId, string toUserId, string message)
        {
            //var fromUserId = Context.ConnectionId;
            var sender = await _context.Users.FindAsync(fromUserId);
            var receiver = await _context.Users.FindAsync(toUserId);

            var mgs = new Message
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Content = Regex.Replace(message, @"<.*?>", string.Empty),
                SendAt = DateTime.Now
            };
            _context.Messages.Add(mgs);
            await _context.SaveChangesAsync();
            var messageViewModel = _mapper.Map<Message, MessageViewModel>(mgs);

            await Clients.All.SendAsync("ReceiveMessagePrivate" + toUserId, messageViewModel);
            await Clients.Caller.SendAsync("ReceiveMessagePrivate", messageViewModel);
        }

        public async Task GetHistoryChatPrivate(string senderId, string receiveId)
        {
            var messages = await _context.Messages.Where(m => m.FromUserId == senderId && m.ToUserId == receiveId || m.FromUserId == receiveId && m.ToUserId == senderId).Include(m => m.FromUser).ToListAsync();
            var messagesViewModel = _mapper.Map<List<Message>, List<MessageViewModel>>(messages);
            await Clients.Caller.SendAsync("ReceiveChatHistoryPrivate", messagesViewModel);
        }

        public async Task SendToRoom(string userId,string roomId, string message)
        {
            try
            {
                int.TryParse(roomId, out int roomIdInt);
                 var room = await _context.Rooms.FindAsync(roomIdInt);
                if (room == null)
                {
                    Console.WriteLine("Room not found!");
                    return;
                }

                Console.WriteLine($"Sending message to room {room.RoomName}...");

                await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomName);
                var user = await _context.Users.FindAsync(userId);

                var mgs = new Message
                {
                    FromUserId = userId,
                    RoomId = roomIdInt,
                    Content = Regex.Replace(message, @"<.*?>", string.Empty),
                    SendAt = DateTime.Now
                };
                _context.Messages.Add(mgs);
                await _context.SaveChangesAsync();
                var messagesVielModel = _mapper.Map<Message, MessageViewModel>(mgs);

                await Clients.Group(room.RoomName).SendAsync("ReceiveMessageRoom" + roomId , messagesVielModel);

                Console.WriteLine("Message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async Task GetHistoryChatRoom(string roomId)
        {
            int.TryParse(roomId, out int roomIdInt);
            var messages = await _context.Messages.Where(m => m.RoomId == roomIdInt).Include(m => m.FromUser).ToListAsync();
            var messagesViewModel = _mapper.Map<List<Message>, List<MessageViewModel>>(messages);
            await Clients.Caller.SendAsync("ReceiveChatHistoryRoom", messagesViewModel);
        }

        public async Task AddUserToRoom(int roomId, string userId)
        {
            var userRoom = new UserRoom
            {
                RoomId = roomId,
                UserId = userId,
                Role = 0
            };
            _context.UserRooms.Add(userRoom);
            await _context.SaveChangesAsync();  

            var room = await _context.Rooms.FindAsync(roomId);
            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomName);
            //await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined {room.RoomName}");
            //await Clients.Caller.SendAsync("ReceiveNotifyJoinANewRoom", "System", $"You joined {room.RoomName}");
            //await Clients.Caller.SendAsync("JoinNewGroup", room);
            await Clients.User(userId).SendAsync("JoinNewGroup", roomViewModel);
            await Clients.All.SendAsync("JoinNewGroup" + userId, roomViewModel);
            await Clients.Group(room.RoomName).SendAsync("ReceiveMessageNewMem", "System", $"{Context.ConnectionId} joined {room.RoomName}");
        }

        public async Task LeaveRoom(int roomId, string userId)
        {
            //int.TryParse(roomId, out int roomIdInt);
            var userRoom = await _context.UserRooms.Where(ur => ur.RoomId == roomId && ur.UserId == userId).FirstOrDefaultAsync();
            _context.UserRooms.Remove(userRoom);
            await _context.SaveChangesAsync();
            var room = await _context.Rooms.FindAsync(roomId);
            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomName);
            await Clients.All.SendAsync("LeaveGroup" + userId, roomViewModel);
            await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} left {room.RoomName}");
        }

        public async Task RemoveUserToRoom(int roomId, string userId)
        {
            var userRoom = await _context.UserRooms.FindAsync(userId, roomId);
            _context.UserRooms.Remove(userRoom);
            await _context.SaveChangesAsync();

            var room = await _context.Rooms.FindAsync(roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomName);
            await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} left {room.RoomName}");
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }
    }
}
