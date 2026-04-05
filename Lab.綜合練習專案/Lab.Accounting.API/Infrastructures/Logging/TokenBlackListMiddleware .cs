using System.IdentityModel.Tokens.Jwt;

namespace Lab.Accounting.API.Infrastructures.Logging
{
    public class TokenBlackListMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlackListMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ITokenBlacklistRepositories tokenBlacklistRepositories
        )
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (tokenHandler.CanReadToken(token))
                {
                    var jwt = tokenHandler.ReadJwtToken(token);
                    var jti = jwt.Id;

                    if (await tokenBlacklistRepositories.isBlackList(jti))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(
                            new { message = "Token 已失效，請重新登入" }
                        );
                        return;
                    }
                }
            }

            await _next(context);
        }

        /// <summary>
        /// 讀取 httpcontext 裡的請求資訊
        /// </summary>
    }
}
