using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken, DateTime expires)> SsoSignInAsync(string provider, string code, string redirectUri, string clientIp);
        Task<(string accessToken, string refreshToken, DateTime expires)> RefreshTokenAsync(string refreshToken, string clientIp);
    }
}
