using Lab.API.Dapper.Models;
using Lab.API.Dapper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _iuserrepository;

        public UserController(IUserRepository repository)
        {
            _iuserrepository = repository;
        }

        //// 登入
        //[AllowAnonymous]
        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(string email, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        //    if (email != user.Email || password != user.Password)
        //    {
        //        return BadRequest();
        //    }

        //    var token = _jwtHelper.GeneratedToken(user.Name, user.Role);
        //    return Ok(token);
        //}

        [HttpGet("All")]
        public async Task<List<UserViewDTO>> GetAllUser()
        {
            return await _iuserrepository.GetAllUsersAsync();
        }

        [HttpGet("UserAndBooks")]
        public async Task<UserAndBooksDTO> GetBooksAndUser([FromQuery] int id)
        {
            return await _iuserrepository.GetBooksAndUser(id);
        }

        [HttpDelete]
        public async Task<bool> DeleteUser([FromQuery] int id)
        {
            var result = await _iuserrepository.DeleteUserAsync(id);
            return result > 0;
        }

        [HttpGet("One")]
        public async Task<UserViewDTO> GetUser([FromQuery] int id)
        {
            return await _iuserrepository.GetUserAsync(id);
        }

        [HttpPut]
        public async Task<bool> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var target = await _iuserrepository.GetUserAsync(dto.Id);
            if (target is null)
            {
                return false;
            }

            var result = await _iuserrepository.UpdateUserAsync(dto);
            return result > 0;
        }

        [HttpPut("UserAndBooksUpdate")]
        public async Task<bool> UpdateUserAndBooks([FromBody] UserUpdateDTO dto)
        {
            var target = await _iuserrepository.GetBooksAndUser(dto.Id);
            if (target is null)
            {
                return false;
            }

            return await _iuserrepository.UpdateUserAndBooks(dto);
        }

        [HttpPost]
        public async Task<int> CreateUser([FromBody] UserInsertDTO dto)
        {
            return await _iuserrepository.InsertUserAsync(dto);
        }

        [HttpPost("InsertUser")]
        public async Task<int> CreateMoreUser()
        {
            return await _iuserrepository.InsertUserTest();
        }

        [HttpPost("InsertChunkUser")]
        public async Task<int> CreateMoreChunkUser()
        {
            return await _iuserrepository.InsertUserChunkTest();
        }

        [HttpPost("InsertBulkUser")]
        public void CreateMoreBolkUser()
        {
            _iuserrepository.InserUserSqlBulkTest();
        }
    }
}
