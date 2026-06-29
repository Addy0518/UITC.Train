namespace Lab.Accounting.API.Common.Helpers;

public class FileUploadHelper
{
    /// <summary>
    /// 儲存實體檔案
    /// </summary>
    /// <param name="file">上傳的檔案</param>
    /// <param name="rootPath">根路徑</param>
    /// <param name="folder">資料夾名稱</param>
    /// <returns>檔案名稱</returns>
    public static async Task<string> SaveFileAsync(IFormFile file, string rootPath, string folder)
    {
        if (file == null || file.Length == 0)
            return null;

        // 拿到檔案名稱 , 並用 Guid 生成唯一識別碼
        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        // 把三個合併成完整路徑 : 根路徑 + 資料夾 + 檔案名稱 , 例如 : wwwroot/images/121212(這時候檔名就會是 Guid ).jpg
        string fullPath = Path.Combine(rootPath, folder, fileName);

        // 確保資料夾存在 , 不存在就創建
        Directory.CreateDirectory(Path.Combine(rootPath, folder));

        // 把檔案寫入磁碟 , FileStream 是用來讀寫檔案的類別 , FileMode.Create 代表如果檔案不存在就創建 , 如果存在就覆蓋
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 回傳檔案名稱 , 這樣前端就可以拿到這個名稱去存資料庫 , 或是用來顯示圖片
        return fileName;
    }

    /// <summary>
    /// 刪除實體檔案
    /// </summary>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="rootPath">根路徑</param>
    /// <param name="folder">資料夾名稱</param>
    /// <returns></returns>
    public static void DeleteFile(string rootPath, string folder, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        string fullPath = Path.Combine(rootPath, folder, fileName);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error deleting file {fullPath}: {ex.Message}");
            }
        }
    }
}
