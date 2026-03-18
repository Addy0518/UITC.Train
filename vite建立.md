# vite 創建

1. 先確認 node 版本是不是 LTS 的 , 最新版有時候會跟 vite 不相容

2. nvm install 24.14.0 => nvm use 24.14.0

3. 安裝 vite => >npm create vite@latest

4. framework 框架選 Vue , variant 用 Official Vue Starter 系統推薦

5. TypeScripts 可以不用 , 後端 Swagger 都有 summary 可以看 api 的設定

6. 功能選擇 : Router（單頁應用程式開發）, Pinia（狀態管理）, Linter（錯誤預防）, Prettier（程式碼格式化）

7. 試驗特性不用 , 就完成了

8. 最後 cd 跳到這個文件底下 , 然後 npm install , 再 npm run format 格式化就行

9. 終端機 npm run dev 就可以啟動 Vue 專案了 , 如果 powershell 說停用指令的話就 => Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned 開啟權限

10. 環境設定都會在 pakagejson , 像 "dev" (開發環境), "build" (建置), "format" (格式化) 都蠻常用

11. 自動整理代碼套件 prettierrc 的設定在設定去編輯 json 就行 (右上角) , "editor.defaultFormatter": "esbenp.prettier-vscode",
    "editor.formatOnSave": true,

12. prettierrc.json 裡可以開啟 "semi": true, 這是開啟 ; 結尾的設定
