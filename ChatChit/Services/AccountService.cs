using ChatChit.Models;
using ChatChit.Services.Interfaces;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ChatChit.ViewModel.AccountViewModel;

namespace ChatChit.Services
{
    public class AccountService : IAccountService
    {
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountService(ITokenService tokenService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<UserViewModel> Login([FromBody] LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByNameAsync(loginViewModel.Username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid password");
            }

            var token = await _tokenService.CreateTokenAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Token = token,
                Avatar = user.Avatar
            };
            return userViewModel;
            //var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginViewModel.Username.ToLower());
            //if (user == null) throw new UnauthorizedAccessException("Invalid username");

            //var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            //if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid password");

            //return new UserViewModel
            //{
            //    Id = user.Id,
            //    UserName = user.UserName,
            //    Token = await _tokenService.CreateTokenAsync(user),
            //    Avatar = user.Avatar
            //};
        }

        public async Task<UserViewModel> Register([FromBody] RegisterViewModel registerViewModel)
        {
            var user = new User
            {
                UserName = registerViewModel.Username,
                DisplayName = registerViewModel.DisplayName,
                Avatar = registerViewModel.Avatar,
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user");
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Avatar = user.Avatar,
                Token = await _tokenService.CreateTokenAsync(user)
            };
            return userViewModel;
        }
    }
}
