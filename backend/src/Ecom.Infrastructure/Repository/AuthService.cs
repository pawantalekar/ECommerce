using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecom.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _users;
        private readonly IRefreshTokenRepository _refresh;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration config, 
            IUserRepository users, 
            IRefreshTokenRepository refresh, 
            IHttpClientFactory httpClientFactory,
            ILogger<AuthService> logger)
        {
            _config = config;
            _users = users;
            _refresh = refresh;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<(string accessToken, string refreshToken, DateTime expires)> SsoSignInAsync(string provider, string code, string redirectUri, string clientIp)
        {
            var info = await ExchangeCodeForUserInfo(provider, code, redirectUri);
            if (info == null) throw new Exception("Invalid SSO");

            var user = await _users.GetBySsoAsync(provider, info.ProviderId);
            if (user == null)
            {
                user = new User
                {
                    Email = info.Email,
                    Name = info.Name,
                    Ssoprovider = provider,
                    SsoproviderId = info.ProviderId,
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _users.AddAsync(user);
            }

            var jwt = GenerateAccessToken(user);
            var refresh = CreateRefreshToken(user.Id, clientIp);
            await _refresh.AddAsync(refresh);

            return (jwt.token, refresh.Token, jwt.expires);
        }

        public async Task<(string accessToken, string refreshToken, DateTime expires)> RefreshTokenAsync(string refreshToken, string clientIp)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Refresh token is null or empty");
                throw new UnauthorizedAccessException("Refresh token is required");
            }

            var normalizedToken = refreshToken.Replace(" ", "+");
            var currentTime = DateTime.UtcNow;

            _logger.LogInformation("Attempting to refresh token. Current UTC time: {CurrentTime}", currentTime);

            var stored = await _refresh.GetByTokenAsync(normalizedToken);
            
            if (stored == null)
            {
                _logger.LogWarning("Refresh token not found in database");
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            _logger.LogInformation(
                "Token found - Created: {Created}, Expires: {Expires}, Revoked: {Revoked}, Current: {Current}",
                stored.CreatedAt, stored.ExpiresAt, stored.RevokedAt, currentTime);

            if (stored.RevokedAt != null)
            {
                _logger.LogWarning("Token was already revoked at {RevokedAt}", stored.RevokedAt);
                throw new UnauthorizedAccessException("Refresh token has been revoked");
            }

            if (stored.ExpiresAt < currentTime)
            {
                _logger.LogWarning(
                    "Token has expired. Expires: {Expires}, Current: {Current}, Difference: {Diff} seconds",
                    stored.ExpiresAt, currentTime, (currentTime - stored.ExpiresAt).TotalSeconds);
                throw new UnauthorizedAccessException("Refresh token has expired");
            }

            var user = stored.User;
            if (user == null)
            {
                _logger.LogError("User not found for token. UserId: {UserId}", stored.UserId);
                throw new UnauthorizedAccessException("User not found");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("User account is inactive. UserId: {UserId}", user.Id);
                throw new UnauthorizedAccessException("User account is inactive");
            }

            _logger.LogInformation("Token validation passed. Generating new tokens for user: {UserId}", user.Id);

            var jwt = GenerateAccessToken(user);
            var newRefresh = CreateRefreshToken(user.Id, clientIp);
            
            await _refresh.AddAsync(newRefresh);
            
            stored.RevokedAt = DateTime.UtcNow;
            await _refresh.SaveChangesAsync();

            _logger.LogInformation("Successfully refreshed token for user: {UserId}", user.Id);

            return (jwt.token, newRefresh.Token, jwt.expires);
        }

        private (string token, DateTime expires) GenerateAccessToken(User user)
        {
            var secret = _config["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret)) throw new Exception("JWT Secret is missing in configuration");

            var key = Encoding.UTF8.GetBytes(secret);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"]));
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: expires, signingCredentials: creds);
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private RefreshToken CreateRefreshToken(Guid userId, string clientIp)
        {
            var random = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(random);
            var expiryMinutes = int.Parse(_config["Jwt:RefreshTokenExpiryMinutes"] ?? "10080"); // Default 7 days
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            _logger.LogInformation(
                "Creating refresh token for user {UserId}. Expires at: {Expires} (in {Minutes} minutes)",
                userId, expires, expiryMinutes);

            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expires,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = clientIp
            };
        }

        private async Task<SsoUserInfo> ExchangeCodeForUserInfo(string provider, string code, string redirectUri)
        {
            if (provider.Equals("google", StringComparison.OrdinalIgnoreCase))
                return await ExchangeCodeGoogle(code, redirectUri);
            if (provider.Equals("microsoft", StringComparison.OrdinalIgnoreCase))
                return await ExchangeCodeMicrosoft(code, redirectUri);
            return null;
        }

        private async Task<SsoUserInfo> ExchangeCodeGoogle(string code, string redirectUri)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenReq = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _config["Authentication:Google:ClientId"]},
                {"client_secret", _config["Authentication:Google:ClientSecret"]},
                {"redirect_uri", redirectUri},
                {"grant_type", "authorization_code"}
            };

            var tokenResp = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenReq));
            if (!tokenResp.IsSuccessStatusCode) return null;

            var tr = await tokenResp.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            if (tr == null || string.IsNullOrEmpty(tr.access_token)) return null;

            var userResp = await client.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={tr.access_token}");
            if (!userResp.IsSuccessStatusCode) return null;

            var user = await userResp.Content.ReadFromJsonAsync<GoogleUserResponse>();
            if (user == null) return null;

            return new SsoUserInfo { Email = user.email, Name = user.name, ProviderId = user.id };
        }

        private async Task<SsoUserInfo> ExchangeCodeMicrosoft(string code, string redirectUri)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenReq = new Dictionary<string, string>
            {
                {"client_id", _config["Authentication:Microsoft:ClientId"]},
                {"client_secret", _config["Authentication:Microsoft:ClientSecret"]},
                {"code", code},
                {"redirect_uri", redirectUri},
                {"grant_type", "authorization_code"},
                {"scope", "openid profile email"}
            };

            var tokenResp = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", new FormUrlEncodedContent(tokenReq));
            if (!tokenResp.IsSuccessStatusCode) return null;

            var tr = await tokenResp.Content.ReadFromJsonAsync<MicrosoftTokenResponse>();
            if (tr == null || string.IsNullOrEmpty(tr.access_token)) return null;

            var req = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tr.access_token);
            var uresp = await client.SendAsync(req);
            if (!uresp.IsSuccessStatusCode) return null;

            var user = await uresp.Content.ReadFromJsonAsync<MicrosoftUserResponse>();
            if (user == null) return null;

            var email = string.IsNullOrEmpty(user.mail) ? user.userPrincipalName : user.mail;
            return new SsoUserInfo { Email = email, Name = user.displayName, ProviderId = user.id };
        }

        private class SsoUserInfo { public string Email { get; set; } public string Name { get; set; } public string ProviderId { get; set; } }
        private class GoogleTokenResponse { public string access_token { get; set; } public string id_token { get; set; } }
        private class GoogleUserResponse { public string id { get; set; } public string email { get; set; } public string name { get; set; } }
        private class MicrosoftTokenResponse { public string access_token { get; set; } }
        private class MicrosoftUserResponse { public string id { get; set; } public string displayName { get; set; } public string mail { get; set; } public string userPrincipalName { get; set; } }
    }
}
