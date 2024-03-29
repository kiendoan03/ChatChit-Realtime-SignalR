using AutoMapper;
using ChatChit.DAL;
using ChatChit.Models;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ChatChit.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        private readonly IMapper _mapper;

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
            await Clients.All.SendAsync("ReceiveMessage", user.DisplayName, message);
        }

        public async Task SendPrivate( string fromUserId,string toUserId, string message)
        {
            //var fromUserId = Context.ConnectionId;
            var mgs = new Message
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Content = Regex.Replace(message, @"<.*?>", string.Empty),
            };
            _context.Messages.Add(mgs);
            await _context.SaveChangesAsync();
            var messageViewModel = _mapper.Map<Message, MessageViewModel>(mgs);
            await Clients.Client(fromUserId).SendAsync("newMessage", messageViewModel.Content);
            await Clients.Caller.SendAsync("newMessage", messageViewModel.Content);
        }

        public async Task SendToRoom(int roomId, string userId, string message)
        {
            var mgs = new Message
            {
                RoomId = roomId,
                FromUserId = userId,
                Content = Regex.Replace(message, @"<.*?>", string.Empty),
            };
            _context.Messages.Add(mgs);
            await _context.SaveChangesAsync();
            var messagesVielModel = _mapper.Map<Message, MessageViewModel>(mgs);
            var user = await _context.Users.FindAsync(userId);
            var room = await _context.Rooms.FindAsync(roomId);
            await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", user.DisplayName, messagesVielModel);
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
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomName);
            await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined {room.RoomName}");
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
    }
}
