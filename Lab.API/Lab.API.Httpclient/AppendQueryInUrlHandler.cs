using System.Web;

namespace Lab.API.Httpclient
{
    // 繼承委派處理器 , 用來處理還沒發出去的請求
    public class AppendQueryInUrlHandler : DelegatingHandler
    {
        // 覆蓋掉原本的請求方法 , 自行修改
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            // 有網址才處理
            if (request.RequestUri is not null)
            {
                // 先把原本的網址傳成字串
                string url = request.RequestUri.ToString();

                // 使用 UriBuilder 工具來拆解網 , 用於修改各個部分
                var uriBuilder = new UriBuilder(url);

                // 把網址中問號後面的參數部分抓出來 , 並解析成一個方便操作的集合
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                // 在參數集合中新增一個 key 叫做 "myQuery , 值設定為 "nitish"
                query["myQuery"] = "nitish";

                // 將改好的參數集合轉回字 , 再塞回原本的網址建構器中
                uriBuilder.Query = query.ToString();

                // 把重新組裝好 , 帶有新參數的完整網址轉回字串
                url = uriBuilder.ToString();

                // 將原本請求中的網址 , 替換成剛剛加工完成的新網址
                request.RequestUri = new Uri(url);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
