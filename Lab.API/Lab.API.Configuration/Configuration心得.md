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

