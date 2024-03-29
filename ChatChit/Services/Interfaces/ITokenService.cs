using ChatChit.Models;

namespace ChatChit.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user);
    }
}
