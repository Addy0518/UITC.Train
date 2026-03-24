# IIS 的建立順序

### 安裝 IIS Hosting Bundle , 這是支援 Net 部屬 IIS 的 => https://dotnet.microsoft.com/en-us/download/dotnet/8.0

1. 先開啟 Window 的內建功能 , 在 控制台 => 程式集 => 開啟關閉 Windows 功能 => 把 Internet Information Services ( IIS的縮寫 ) , 裡的 Web 工具跟服務都打開 , 詳細看這裡 => https://www.ajengcodingnotes.com/how-to-install-iis-in-windows-11/

2. .Net 要部屬的檔案 發布 => 選擇資料夾 => 版本 => 直接發布

3. 完成後重新啟動 , 進去後新增站台並啟動集區來驅動站台 

4. 站台可以先選擇一個空資料夾 , 並開啟這個資料夾的 內容 => 安全性 => 這台電腦 => 新增 IUSER 跟 ISS_User , 這是IIS對於這個資料夾的權限

5. 可以先新增一個html來測試看看 , 確認沒問題再把發布的程式放進來 , 因為每次直接發布到這個資料夾可能需要重新設定權限 , 所以可以發布到另一個資料夾然後丟進來

6. 最後啟動並測試 , 記得 SQL 要用 Sql server 連線才能有權限

### 前端的 IIS 部屬

#### 因為我部署的是單網頁檔案 ( 只有 index.html ) , 所以當 IIS 啟動時 , 他會先讀取 index 並成功 , 但當我換其他路由 (比如 inex/test) 重整時 , 他會對 server 端發送請求 , 但我們檔案只有 index 一個 , 他就會報錯誤 , 他沒辦法搜尋再 index 裡面的 test 子路由

1. 所以要先解決這個問題 , 先安装 IIS UrlRewrite , 用來幫助 IIS 找路由

2. 並在我們要部屬的網站下建立一個 web.config 檔案讓 IIS 讀取 , 用來設定路由

```html
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Handle History Mode and custom 404/500" stopProcessing="true">
          <match url="(.*)" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <!-- 這就是用來讓 IIS 知道要重新找哪個路由 -->
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```　

3. 前端的話直接去 pakage.json 部屬 build 就好 , 就可以在 dist 看到 html , css ,js 三件套