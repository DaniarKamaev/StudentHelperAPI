using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using StudentHelperAPI.Models;
using System.Security.Claims;
using System.Text;

namespace StudentHelperAPI.Features.Authentication.Auth
{
    public class AuthHandler : IRequestHandler<AuthRequest, AuthResponse>
    {
        private readonly HelperDbContext _db;
        private readonly IConfiguration _configuration;
        public AuthHandler(HelperDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Handle(AuthRequest request, CancellationToken cancellationToken)
        {
            var userMain = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == request.email, cancellationToken);

            if (userMain == null)
                return new AuthResponse(null, false, "Нет такого пользователя");

            bool isValidPassword = HashCreater.VerifyPassword(request.password, userMain.PasswordHash);
            if (isValidPassword)
            {
                var token = GenerateJwtToken(userMain);
                return new AuthResponse(token, true, "Вход успешо выполнен");
            }
            return new AuthResponse(null, false, "Неверный логин или пароль");
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserRole", user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
