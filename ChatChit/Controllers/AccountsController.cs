using ChatChit.Models;
using ChatChit.Services.Interfaces;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static ChatChit.ViewModel.AccountViewModel;

namespace ChatChit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserViewModel>> Login(LoginViewModel loginViewModel)
        {
            return await _accountService.Login(loginViewModel);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> Register(RegisterViewModel registerViewModel)
        {
           return await _accountService.Register(registerViewModel);
        }
    }
}
