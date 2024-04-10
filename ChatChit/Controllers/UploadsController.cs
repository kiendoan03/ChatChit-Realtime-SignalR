using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using ChatChit.DAL;
using ChatChit.Helpers;
using ChatChit.Hubs;
using ChatChit.Models;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using static ChatChit.ViewModel.UploadViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatChit.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowedExtensions;
        private readonly ChatDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileValidator _fileValidator;

        public UploadsController(ChatDbContext context,
             IMapper mapper,
             IWebHostEnvironment environment,
             IHubContext<ChatHub> hubContext,
             IConfiguration configuration,
             IFileValidator fileValidator)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
            _hubContext = hubContext;
            _fileValidator = fileValidator;

            FileSizeLimit = configuration.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowedExtensions = configuration.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
        }

        [HttpPost]
        public async Task<IActionResult> UploadToRoom([FromForm] UploadViewModelToRoom viewModelToRoom)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModelToRoom.File))
                    return BadRequest("Validation failed!");

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModelToRoom.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModelToRoom.File.CopyToAsync(fileStream);
                }

                var room = _context.Rooms.Where(r => r.Id == viewModelToRoom.RoomId).FirstOrDefault();

                string htmlImage = string.Format(
                    "<a href=\"https://localhost:7014/uploads/{0}\" target=\"_blank\">" +
                    "<img src=\"https://localhost:7014/uploads/{0}\" class=\"post-image\">" +
                    "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToRoom.FromUserId,
                    RoomId = viewModelToRoom.RoomId
                };
                var user = await _context.Users.FindAsync(viewModelToRoom.FromUserId);

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.Group(room.RoomName).SendAsync("ReceiveMessageRoom" + viewModelToRoom.RoomId, messageViewModel);

                return Ok(messageViewModel.Content);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UploadToUser([FromForm] UploadViewModelToUser viewModelToUser)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModelToUser.File))
                    return BadRequest("Validation failed!");

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModelToUser.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModelToUser.File.CopyToAsync(fileStream);
                }

                var toUser = _context.Users.Where(u => u.Id == viewModelToUser.ToUserId).FirstOrDefault();

                string htmlImage = string.Format(
                                       "<a href=\"https://localhost:7014/uploads/{0}\" target=\"_blank\">" +
                                                          "<img src=\"https://localhost:7014/uploads/{0}\" class=\"post-image\">" +
                                                                             "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToUser.FromUserId,
                    ToUserId = viewModelToUser.ToUserId
                };
                var user = await _context.Users.FindAsync(viewModelToUser.FromUserId);

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
                var httpContext = HttpContext;
                var connectionId = httpContext.Connection.Id;
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                //await _hubContext.Clients.User(toUser.UserName).SendAsync("newMessage", messageViewModel);
                await _hubContext.Clients.All.SendAsync("ReceiveMessagePrivate" + viewModelToUser.ToUserId, messageViewModel);
                await _hubContext.Clients.All.SendAsync("ReceiveMessagePrivate" + viewModelToUser.FromUserId, messageViewModel);
                //await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessagePrivate", messageViewModel);

                return Ok(messageViewModel.Content);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UploadToLobby([FromForm] UploadViewModelToLobby viewModelToLobby)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModelToLobby.File))
                    return BadRequest("Validation failed!");

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModelToLobby.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModelToLobby.File.CopyToAsync(fileStream);
                }

                string htmlImage = string.Format(
                                       "<a href=\"https://localhost:7014/uploads/{0}\" target=\"_blank\">" +
                                                          "<img src=\"https://localhost:7014/uploads/{0}\" class=\"post-image\">" +
                                                                             "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToLobby.FromUserId
                };
                var user = await _context.Users.FindAsync(viewModelToLobby.FromUserId);

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageViewModel);
                //await Clients.All.SendAsync("ReceiveMessage", messageViewModel);

                return Ok(messageViewModel.Content);
            }

            return BadRequest();
        }
    }
}
