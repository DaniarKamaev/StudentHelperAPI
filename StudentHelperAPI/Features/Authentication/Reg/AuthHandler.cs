
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudentHelperAPI.Models;
using System.Text.RegularExpressions;

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
            var groupId = await _db.StudentGroups.FirstOrDefaultAsync(x => x.Name == request.GrupId);
            //if (groupId == null)
              //  return new AuthResponse(Guid.Parse("00000000-0000-0000-0000-000000000001"), false , "Такой группы нету");

            //TODO Валидация группы
            var password = HashCreater.HashPassword(request.password);
            var user = new Models.User
            {
                Email = request.email,
                PasswordHash = password,
                LastName = request.lastName,
                FirstName = request.firstNamem,
                Role = request.role,
                GroupId = groupId.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _db.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new AuthResponse(user.Id, true, "Пользователь успешно зарегистрирован");
        }
    }
}
