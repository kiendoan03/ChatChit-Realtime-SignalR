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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using static ChatChit.ViewModel.UploadViewModel;

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
        //[ValidateAntiForgeryToken]
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

                //var user = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var room = _context.Rooms.Where(r => r.Id == viewModelToRoom.RoomId).FirstOrDefault();
                //if (user == null || room == null)
                //    return NotFound();

                string htmlImage = string.Format(
                    "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                    "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToRoom.FromUserId,
                    RoomId = viewModelToRoom.RoomId
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // Send image-message to group
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.Group(room.RoomName).SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
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

                //var user = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var toUser = _context.Users.Where(u => u.Id == viewModelToUser.ToUserId).FirstOrDefault();
                //if (user == null || toUser == null)
                //    return NotFound();

                string htmlImage = string.Format(
                                       "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                                                          "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                                                                             "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToUser.FromUserId,
                    ToUserId = viewModelToUser.ToUserId
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // Send image-message to user
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.User(toUser.UserName).SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
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

                //var user = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                //if (user == null)
                //    return NotFound();

                string htmlImage = string.Format(
                                       "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                                                          "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                                                                             "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    SendAt = DateTime.Now,
                    FromUserId = viewModelToLobby.FromUserId
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // Send image-message to all
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.All.SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }
    }
}
