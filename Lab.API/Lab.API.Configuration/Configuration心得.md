# Configuration 的學習心得



## 設定資料

1. ### Sercret.json 的層級是最高的 , 再來是 appsettings.Development.json (開發環境) 最後是 appsettings.json (預設環境) , 通常都使用 appsettings做設定 , Sercret.json 是當今天有不同的人想設定其他條件就可以在這裡設定並覆蓋本來的設定
```csharp


// 像連線字串 , API KEY 等等就可以放在這裡 , 因為 setting 也不會傳上去
{
  "ConnectionStrings": {
    "DBConStr": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AspCoreIThelp2020;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  },
  {
    "OpenAPIKey": "The_API_Key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "D"
}

// 在 Progarm 再去讀取設定
var defaultConnectionString =
   builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
```

2. ### editorconfig 設定檔可以自訂編輯 , 可以在這裡設定固定的編碼格式 ( UTF-8 ) , 或者是程式碼格式設定等等 , 只要安裝套件 editorconfig 並且開啟 , 就能直接用 UI 操作 , 或是用寫的也可以

```csharp

// 全檔案設定 , 命名規則等等 . . .
# All files
[*]
charset = utf-8
indent_style = space
end_of_line = crlf

# Xml files
[*.xml]
indent_size = 2

[*.cs]
#### 命名樣式 ####

# 命名規則

dotnet_naming_rule.interface_should_be_begins_with_i.severity = suggestion
dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i


```


3. appsettings.json 讀取


```csharp

{
    "ConnectionStrings": {
        // 連線字串
        "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=SQL"
    },
    "OpenAPIKey": "The_API_Key",
    "ApiSettings": {
        // Key1
        "ApiOne": "OneKey",
        // Key2
        "ApiTwo": "TwoKey"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*"
}


 [HttpGet]
 public IActionResult Get()
 {
     
     var Api1 = _configuration["ApiSettings:ApiOne"];
     var Api2 = _configuration["ApiSettings:ApiTwo"];

     var constr = _configuration.GetConnectionString("DefaultConnection");

     return Ok(
         new
         {
             constr,
             Api1,
             Api2,
         }
     );
 }

```


4. Options Pattern 使用


```csharp

// 設定值
"StrongholdInfo": {
    "Index": 49,
    "Name": "劍閣",
    "Enabled": true,
    "General": [
        "姜維",
        "廖化",
        "張翼",
        "董厥"
    ]
},

// 建立類別放設定值
public class StrongholdInfoOptions
{

    public int Index { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public string[]? General { get; set; }
}

// 用 Configure 註冊並且型別是剛剛創建的 StrongholdInfoOptions
builder.Services.Configure<StrongholdInfoOptions>(
    // 再用 GetSection 指定內容是在 appsetting 的 StrongholdInfo
    builder.Configuration.GetSection("StrongholdInfo")
);


// 新增一個剛剛創建的類別
private readonly StrongholdInfoOptions _Info;


// 依賴 StrongholdInfoOptions 注入 , 用 IOptions 取得內容
public SampleController(
    IConfiguration configuration,
    IOptions<StrongholdInfoOptions> options
)
{
    _configuration = configuration;
    _Info = options.Value;
}


[HttpGet("IOption")]
public Object GetInfo()
{
    return _Info;
}



```