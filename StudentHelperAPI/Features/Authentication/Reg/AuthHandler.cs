
using MediatR;
using StudentHelperAPI.Models;

namespace StudentHelperAPI.Features.Authentication.Reg
{
    public class AuthHandler : IRequestHandler<AuthRequest, AuthResponse>
    {
        private readonly HelperDbContext _db;
        public AuthHandler(HelperDbContext db)
        {
            _db = db;
        }

        public async Task<AuthResponse> Handle(AuthRequest request, CancellationToken cancellationToken)
        {
            var password = HashCreater.HashPassword(request.password);
            var user = new User
            {
                Email = request.email,
                PasswordHash = password,
                LastName = request.lastName,
                FirstName = request.firstNamem,
                Role = request.role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _db.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new AuthResponse(user.Id, true, "Пользователь успешно зарегистрирован");
        }
    }
}
