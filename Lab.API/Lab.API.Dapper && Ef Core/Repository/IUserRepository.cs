using Lab.API.Dapper.Models;

namespace Lab.API.Dapper.Repository
{
    public interface IUserRepository
    {
        Task<int> InsertUserAsync(UserInsertDTO userDto);

        Task<int> InsertUserTest();

        Task<int> InsertUserChunkTest();

        void InserUserSqlBulkTest();

        Task<List<UserViewDTO>> GetAllUsersAsync();

        Task<UserViewDTO> GetUserAsync(int id);

        Task<int> UpdateUserAsync(UserUpdateDTO user);

        Task<bool> UpdateUserAndBooks(UserUpdateDTO dto);

        Task<UserAndBooksDTO> GetBooksAndUser(int id);

        Task<int> DeleteUserAsync(int id);
    }
}
