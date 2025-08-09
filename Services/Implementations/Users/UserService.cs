using Education.Api.Data;
using Education.Api.Dtos.Users;
using Education.Api.Services.Abstractions.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Users;

public class UserService(ApplicationDbContext context) : IUserService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<UserDto> GetByIdAsync(int id)
    {
        return await _context
                .Users
                .Select(
                    u =>
                        new UserDto
                        {
                            Id = u.Id,
                            Username = u.Username,
                            Email = u.Email,
                            Role = u.Role,
                            IsVerified = u.IsVerified,
                            CreatedAt = u.CreatedAt
                        }
                )
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"User with ID ""{id}"" does not exist");
    }
}
