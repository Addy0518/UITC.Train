# IIS 的建立順序

### 安裝 IIS Hosting Bundle , 這是支援 Net 部屬 IIS 的 => https://dotnet.microsoft.com/en-us/download/dotnet/8.0

1. 先開啟 Window 的內建功能 , 在 控制台 => 程式集 => 開啟關閉 Windows 功能 => 把 Internet Information Services ( IIS的縮寫 ) , 裡的 Web 工具跟服務都打開 , 詳細看這裡 => https://www.ajengcodingnotes.com/how-to-install-iis-in-windows-11/

2. .Net 要部屬的檔案 發布 => 選擇資料夾 => 版本 => 直接發布

3. 完成後重新啟動 , 進去後新增站台並啟動集區來驅動站台 

4. 站台可以先選擇一個空資料夾 , 並開啟這個資料夾的 內容 => 安全性 => 這台電腦 => 新增 IUSER 跟 ISS_User , 這是IIS對於這個資料夾的權限

5. 可以先新增一個html來測試看看 , 確認沒問題再把發布的程式放進來 , 因為每次直接發布到這個資料夾可能需要重新設定權限 , 所以可以發布到另一個資料夾然後丟進來

6. 最後啟動並測試 , 記得 SQL 要用 Sql server 連線才能有權限