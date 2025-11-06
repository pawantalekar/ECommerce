using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IConfiguration _config;

        public AuthController(IAuthService auth, IConfiguration config)
        {
            _auth = auth;
            _config = config;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var clientId = _config["Authentication:Google:ClientId"];
            var redirectUri = "https://localhost:7024/api/Auth/google-response";
            var scope = "openid profile email";
            var state = Guid.NewGuid().ToString("N");
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}";
            return Redirect(url);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest(new { message = "No code provided. You must start login via /google-login" });

            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _auth.SsoSignInAsync("google", code, "https://localhost:7024/api/Auth/google-response", clientIp);

            return Ok(new TokenResponse
            {
                AccessToken = result.accessToken,
                RefreshToken = result.refreshToken,
                Expires = result.expires
            });
        }


        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _auth.RefreshTokenAsync(request.RefreshToken, clientIp);
            return Ok(new TokenResponse
            {
                AccessToken = result.accessToken,
                RefreshToken = result.refreshToken,
                Expires = result.expires
            });
        }
    }
}
