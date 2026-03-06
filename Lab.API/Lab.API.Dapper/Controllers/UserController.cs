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
        private readonly TestContext _context;

        public UserController(IUserRepository repository, TestContext context)
        {
            _iuserrepository = repository;
            _context = context;
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

        [HttpPost]
        public async Task<int> CreateUser([FromBody] UserInsertDTO dto)
        {
            return await _iuserrepository.InsertUserAsync(dto);
        }
    }
}
