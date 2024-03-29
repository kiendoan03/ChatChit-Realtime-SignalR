using ChatChit.ViewModel;
using Microsoft.AspNetCore.Mvc;
using static ChatChit.ViewModel.AccountViewModel;

namespace ChatChit.Services.Interfaces
{
    public interface IAccountService
    {
        Task<UserViewModel> Login([FromBody] LoginViewModel loginViewModel);
        Task<UserViewModel> Register([FromBody] RegisterViewModel registerViewModel);
    }
}
