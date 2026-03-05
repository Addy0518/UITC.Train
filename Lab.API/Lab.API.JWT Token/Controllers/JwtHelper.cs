using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab.API.JWT_Token.Models;
using Microsoft.IdentityModel.Tokens;

namespace Lab.API.JWT_Token.Controllers
{
    // 建立一個 Helper 來實作
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        // 注入設定
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 創建 Token 方法
        public string GeneratedToken(string username, int expireMinutes = 30)
        {
            // 拿到設定檔的發行人跟鑰匙
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            // 創建一個放 Token 內容的集合
            var claims = new List<Claim>();

            // 加入使用者姓名
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, username));

            // 加入 Jti 防止重複
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            // 加入角色權限
            claims.Add(new Claim("role", "Admin"));

            // 合併成角色資訊
            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立對稱式金鑰 , 用於給 Jwt 簽名
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // 使用 HmacSha256 雜湊處理產生唯一的密鑰
            var signingCredentials = new SigningCredentials(
                securitykey,
                SecurityAlgorithms.HmacSha256Signature
            );

            // 建立完整 Token 內容
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials,
            };

            // 最後產出 Jwt 物件 , 並序列化成字串
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
