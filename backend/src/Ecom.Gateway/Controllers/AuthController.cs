using AuthService.Api.Commands.AddAddress;
using AuthService.Api.Commands.DeleteAddress;
using AuthService.Api.Commands.UpdateAddress;
using AuthService.Api.Commands.UpdateProfile;
using AuthService.Api.Queries;
using AuthService.Api.Queries.GetAddress;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Ecom.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refresh;
        private readonly IMediator _mediator;

        public AuthController(IAuthService auth, IConfiguration config, IRefreshTokenRepository refresh, IMediator _mediator)
        {
            _auth = auth;
            _config = config;
            _refresh = refresh;
            this._mediator = _mediator;
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
            if (string.IsNullOrEmpty(code)) return BadRequest();

            var result = await _auth.SsoSignInAsync("google", code, "https://localhost:7024/api/Auth/google-response", "unknown");

            var url = $"http://localhost:4200/auth/callback" +
                      $"?accessToken={result.accessToken}" +
                      $"&refreshToken={result.refreshToken}" +
                      $"&expires={result.expires:o}";

            return Redirect(url);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] JsonElement body)
        {
            string? refreshToken = null;

            if (body.ValueKind != JsonValueKind.Undefined && body.ValueKind != JsonValueKind.Null)
            {
                if (body.TryGetProperty("refreshToken", out var prop))
                    refreshToken = prop.GetString();
                else if (body.TryGetProperty("refreshToken", out prop))
                    refreshToken = prop.GetString();
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var tokenEntity = await _refresh.GetByTokenAsync(refreshToken);
                if (tokenEntity != null && tokenEntity.RevokedAt == null)
                {
                    await _refresh.RevokeAsync(tokenEntity, "User logout");
                }
            }

            return Ok(new { success = true });
        }

        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            try
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while refreshing the token" });
            }
        }
        
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var role = await _mediator.Send(new GetCurrentUserRoleQuery());
            return Ok(new { role });
        }
        [HttpGet("myprofile")]
        [Authorize]
        public async Task<IActionResult> GetUSersProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyProfileQuery(),cancellationToken);
            return Ok(new { result });
        }
        [HttpPut("updateprofile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        [HttpGet("addresses")]
        [Authorize]
        public async Task<IActionResult> GetMyAddresses()
        {
            var addresses = await _mediator.Send(new GetAddressesQuery());
            return Ok(addresses);
        }

        [HttpPost("addresses")]
        [Authorize]
        public async Task<IActionResult> AddMyAddress([FromBody] AddAddressCommand command)
        {
            var newAddress = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMyAddresses), new { id = newAddress.Id }, newAddress);
        }
        [HttpPut("addresses/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMyAddress([FromRoute] Guid id, [FromBody] UpdateAddressCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new { success = result });
        }
        [HttpDelete("addresses/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMyAddress([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
            return Ok(new { success = result });
        }
    }
}
