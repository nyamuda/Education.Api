using Education.Api.Dtos.Users;

namespace Education.Api.Services.Abstractions.Users;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(int id);
}
