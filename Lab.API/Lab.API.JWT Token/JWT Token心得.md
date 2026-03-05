# JWT Token 心得


### Token 主要步驟是 : 產生 Token => 驗證 Token => API 驗證 


1. 先在 appsetting 註冊 JWT Token , 這樣製作 Token 的時候就直接讀取這裡就好 

```csharp
"JwtSettings": {
  // 發行人
  "Issuer": "IMAC",
  // Token 簽名 , 要超過 256 位元才行
  "SignKey": "bJs3iqzDSP1qiTzWeMJa2cMsQFji2q6DL5exm0wVKo21NczRvpfE5m7oUE1VCp4F",
  // 過期時間
  "ExpireMinutes": 720 
},    
```

2. 創建 Token   

```csharp
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
        claims.Add(new Claim("roles", "Admin"));

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
```

3. 在 Program 註冊套件 JwtBearer , 讓系統認得出 Bearer Token

```csharp
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 有錯誤就會顯示詳細原因
        options.IncludeErrorDetails = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 發行人
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
            // 接收者
            ValidateAudience = false,
            ValidAudience = "JwtAuthDemo",
            // Token 的有效期間
            ValidateLifetime = true,
            // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
            ValidateIssuerSigningKey = false,
            // key
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration.GetValue<string>("JwtSettings:SignKey")
                )
            ),
        };
    });

// 記得使用這個方法
builder.Services.AddSingleton<JwtHelper>();

```

4. 在 controller 就可以掛上 [Authorize] 標籤 , 讓這個 cotroller 的 API 都需要 Token 驗證 , 寫一個製造 Token 的方法 , 就可以掛上 [AllowAnonymous] , 因為這個 API 不用驗證

```csharp
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
```

5. 在寫一些 API 確認 Token 內容 , 這裡我分成類別跟內容 , 因為 JWT 把類別縮寫 , 所以我上網找了每個對應的類別

```csharp
  // 實際查看 Token 的內容
  [HttpGet("~/claims")]
  public IActionResult GetClaims()
  {
      return Ok(User.Claims.Select(p => new { p.Type, p.Value }));
  }
```
| 欄位名稱      | 說明 |
| ----------- | ----------- |
| Jti      | 表示 Issuer，發送 Token 的發行者       |
| Iss   | 表示 Issuer，發送 Token 的發行者        |
| Iat      | 表示 Issued At，Token 的建立時間     |
| Exp   | 表示 Expiration Time，Token 的逾期時間        |
| Sub      | 表示 Subject，Token 的主體內容     |
| Aud   | 表示 Audience，接收 Token 的觀眾      |
| Typ      | 	表示 Token 的類型，例如 JWT 表示 JSON Web Token 類型   |
| Nbf   | 表示 Not Before，定義在什麼時間之前，不可用        |
| Actort   | 識別執行授權的代理是誰        |

6. 也可以單獨查像是角色權限等等

```csharp
[HttpGet("~/role")]
public IActionResult GetRole()
{
    var role = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role);
    return Ok(role.Value);
}
```
     