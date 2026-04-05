using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lab.Accounting.API.Infrastructures.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Lab.Accounting.API.Common.Helpers
{
    public class TokenHelper(IConfiguration _configuration)
    {
        // 創建 Token 方法
        public string GeneratedToken(int userId, string username, int expireMinutes = 30)
        {
            // 拿到設定檔的發行人跟鑰匙
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            // 創建一個放 Token 的 key-value 資訊
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", userId.ToString()));

            // 加入使用者姓名
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, username));

            // 加入 Jti ( 唯一碼 ) 防止重複
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            // 合併成角色資訊
            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立之前在 appsetting 設定的金鑰 , 用同一把金鑰加密跟解密
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

            // 最後產出 Jwt 物件 ( JwtSecurityTokenHandler ) , 負責產生 , 解析 , 驗證 , 並序列化成字串
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
