using Lab.API.Dapper.Models;

namespace Lab.API.Dapper.Repository
{
    public interface IUserRepository
    {
        Task<int> InsertUserAsync(UserInsertDTO userDto);

        Task<List<UserViewDTO>> GetAllUsersAsync();

        Task<UserViewDTO> GetUserAsync(int id);

        Task<int> UpdateUserAsync(UserUpdateDTO user);

        Task<int> DeleteUserAsync(int id);
    }
}
