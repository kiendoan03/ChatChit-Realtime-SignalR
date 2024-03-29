using AutoMapper;
using ChatChit.DAL;
using ChatChit.Hubs;
using ChatChit.Models;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatChit.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    
    public class RoomsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomsController(ChatDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }
       
        [HttpGet] 
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> GetRooms()
        {
            var rooms = await _context.Rooms
                 .Include(r => r.Admin)
                 .ToListAsync();

            var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomViewModel>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);

            return Ok(roomViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<RoomViewModel>> PostRoom(RoomViewModel roomViewModel)
        {
            var claimsPrincipal = HttpContext.User;
            if (_context.Rooms.Any(r => r.RoomName == roomViewModel.RoomName))
                return BadRequest("Invalid room name or room already exists");
            var userIdClaim = claimsPrincipal.FindFirst("UserId");
            var userId = userIdClaim.Value;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var room = new Room() { RoomName = roomViewModel.RoomName, Admin = user };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var userRoom = new UserRoom() { RoomId = room.Id, UserId = user.Id , Role = 1};
            _context.UserRooms.Add(userRoom);
            await _context.SaveChangesAsync();

            var createdRoom = _mapper.Map<Room, RoomViewModel>(room);
            await _hubContext.Clients.All.SendAsync("addChatRoom", createdRoom);

            return CreatedAtAction(nameof(GetRooms), new { id = room.Id }, createdRoom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, RoomViewModel roomViewModel)
        {
            if (_context.Rooms.Any(r => r.RoomName == roomViewModel.RoomName))
                return BadRequest("Invalid room name or room already exists");

            var room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            room.RoomName = roomViewModel.RoomName;
            await _context.SaveChangesAsync();

            var updatedRoom = _mapper.Map<Room, RoomViewModel>(room);
            await _hubContext.Clients.All.SendAsync("updateChatRoom", updatedRoom);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.RoomName).SendAsync("onRoomDeleted");

            return Ok();
        }
    }
}
