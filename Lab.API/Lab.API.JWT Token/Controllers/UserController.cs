using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab.API.JWT_Token.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Lab.API.JWT_Token.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly TestContext testContext;
        private readonly JwtHelper _jwtHelper;

        public UserController(
            IConfiguration configuration,
            TestContext context,
            JwtHelper jwtHelper
        )
        {
            _configuration = configuration;
            testContext = context;
            _jwtHelper = jwtHelper;
        }

        // 產生Token
        [AllowAnonymous]
        [HttpPost("~/gettoken")]
        public IActionResult GetToken(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }
            var token = _jwtHelper.GeneratedToken(username);
            return Ok(token);
        }

        // 實際查看 Token 的內容
        [HttpGet("~/claims")]
        public IActionResult GetClaims()
        {
            return Ok(User.Claims.Select(p => new { p.Type, p.Value }));
        }

        // 回傳我們剛剛在產Token時輸入的username
        [HttpGet("~/username")]
        public IActionResult GetUserName()
        {
            return Ok(User.Identity.Name);
        }

        // 傳回Jwt的id
        [HttpGet("~/jwtid")]
        public IActionResult GetUniqueId()
        {
            var jti = User.Claims.FirstOrDefault(p => p.Type == "jti");
            return Ok(jti.Value);
        }

        // 也可以單獨查像是角色權限等等
        [HttpGet("~/role")]
        public IActionResult GetRole()
        {
            var role = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role);
            return Ok(role.Value);
        }
    }
}
