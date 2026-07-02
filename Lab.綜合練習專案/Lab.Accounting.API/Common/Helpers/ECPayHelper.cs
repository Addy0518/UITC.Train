namespace prjGonowWebApi.Areas.Company.Helper;

public class ECPayHelper
{
    /// <summary>
    /// 製作金流檢查瑪並附上這筆交易 ( 類似識別證 )
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>檢查碼</returns>
    public static string GetCheckMacValue(Dictionary<string, string> order)
    {
        //排序參數,讓參數依照字母A-Z排序,為了對應雙方的加密字串
        //他就是字典(Dictionary)的作用
        //key代表的是單字(例如TotalAmount金額,ID等等)
        //而order[key]就會是這個單字的解釋,比如金額是1000,id是50
        //組合起來就變成TotalAmount=1000
        var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

        //這裡把是把&加入到字串,用$區分開參數,param就是我們剛剛建立的字典
        //如果沒有加入&就變成=>TotalAmount=1000id=50
        //有的話就是=> TotalAmount = 1000&id = 50,把他們區分開來
        string checkValue = string.Join("&", param);

        //HashKey跟HashTV是商品的金鑰,通常是不能外流的,不過這裡是串接測試用得,就直接寫死
        var hashKey = "pwFHCqoQZGmho4w6";
        var hashIV = "EkRm7iFT261dpevs";
        //再把他加入到加密字串(加到字串的頭跟尾)
        checkValue = $"HashKey={hashKey}&{checkValue}&HashIV={hashIV}";

        //URL 編碼：將特殊字元轉為 %xx 格式，並轉成小寫
        //這部分的用意是,當今天我的單詞是Id & TourId,這個&是單詞,但是系統會以為這是要區分成兩個
        //用Url編碼就會把詞裡面的特殊符號變詞的一部分,也能統一格式
        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

        // 5. 執行 RFC 1866 特定字元替換 (這是成功的關鍵,比較嚴格,他會要求特殊編碼轉成符號)
        //例如把%20轉成+
        checkValue = checkValue
            .Replace("%20", "+")
            .Replace("%2d", "-")
            .Replace("%5f", "_")
            .Replace("%2e", ".")
            .Replace("%21", "!")
            .Replace("%2a", "*")
            .Replace("%28", "(")
            .Replace("%29", ")");

        //雜湊=>把最終結果丟進SHA256演算法,讓她變成一串亂碼,這就是最終的檢查碼
        return GetSHA256(checkValue).ToUpper();
    }

    /// <summary>
    /// 製作物流檢查碼 ( MD5，物流 API 使用，跟金流的 SHA256 不同 )
    /// </summary>
    /// <param name="order">物流參數</param>
    /// <param name="hashKey">物流 HashKey</param>
    /// <param name="hashIV">物流 HashIV</param>
    public static string GetCheckMacValueMD5(Dictionary<string, string> order, string hashKey, string hashIV)
    {
        // 前面這幾步跟金流版本完全一樣
        var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
        string checkValue = string.Join("&", param);
        checkValue = $"HashKey={hashKey}&{checkValue}&HashIV={hashIV}";

        checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
        checkValue = checkValue
            .Replace("%20", "+")
            .Replace("%2d", "-")
            .Replace("%5f", "_")
            .Replace("%2e", ".")
            .Replace("%21", "!")
            .Replace("%2a", "*")
            .Replace("%28", "(")
            .Replace("%29", ")");

        // 只有這裡不同：換成 MD5
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(checkValue));
        return BitConverter.ToString(hash).Replace("-", "").ToUpper();
    }

    /// <summary>
    /// 把檢查碼加密 ( SHA256 )
    /// </summary>
    /// <param name="value">檢查碼</param>
    /// <returns>加密後的檢查碼</returns>
    private static string GetSHA256(string value)
    {
        //建立一個轉成字串變數,待會用於把亂碼轉成字串
        var result = new StringBuilder();
        //接下來要把新建里的Sha256這個變數轉乘sha356加密,用using預防做到一半失敗,結果失敗的資料佔記憶體
        using (var sha256 = SHA256.Create())
        {
            //電腦要加密之前要先看懂,但純文字她看不懂,所以Encoding.UTF8先把它至轉成0跟1
            var bts = Encoding.UTF8.GetBytes(value);
            //sha256.ComputeHash是一個類似魔術盒的概念,他會把丟進去的0跟1轉成一串二進位亂碼
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                //但是二進位太難讀了,所以我們再把他一個字一個自用迴圈讀取並轉成16進位(X2)
                //1.為何要轉16進位?因為2進位裡有一些符號是會給電腦下指令的(比如數字 13 是換行)
                //而16進位全部字元都是可列印,也就是不管傳過去的瑪有多亂,他都能整齊轉成一排字元
                //2.而且其實不僅是電腦要讀取,我們也要讀,當綠界回傳檢查碼錯誤時,我們要比對雙方的字串哪裡有誤
                result.Append(hash[i].ToString("X2"));
            }
        }
        //最後在用剛剛的StringBuilder轉成字串
        return result.ToString();
    }
}
