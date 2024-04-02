using ChatChit.Services.Interfaces;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatChit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers()
        {
            return Ok(await _userService.GetAllUser());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [HttpGet]
        [Route("GetUserByName")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUserByName(string name)
        {
            return Ok(await _userService.GetUserByName(name));
        }

        [HttpGet]
        [Route("GetUserByPhone")]
        public async Task<ActionResult<UserViewModel>> GetUserByPhone(string phone)
        {
            return Ok(await _userService.GetUserByPhone(phone));
        }

        [HttpGet]
        [Route("GetUserNotInGroup")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUserNotInGroup(int groupId)
        {
            return Ok(await _userService.GetUserNotInGroup(groupId));
        }

    }
}
