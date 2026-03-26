namespace Lab.Accounting.API.Infrastructures.Logging
{
    public class ResponseRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // 用做好的讀取 stream 方法
            string requestBodyPayload = await ReadRequestBody(context.Request);
            // 塞回 request 內部
            context.Items["RequestBody"] = requestBodyPayload;

            // response 也是一樣概念 , 先標記 Body
            var orginalresponse = context.Response.Body;

            // 給他複製一個新物件 stream , 塞進 body
            using (var responsebody = new MemoryStream())
            {
                context.Response.Body = responsebody;
                // 往下一層走
                await _next(context);
                string rsponseBodyPayload = await ReadResponseBody(context.Response);
                // 再把原本的 stream 塞回去 , 真的進來被我們變成複製的 , 要出去時再把原本的塞回去的概念
                context.Items["ResponseBody"] = rsponseBodyPayload;
                responsebody.Position = 0;
                await responsebody.CopyToAsync(orginalresponse);
                context.Response.Body = orginalresponse;
            }
        }

        /// <summary>
        /// 讀取 httpcontext 裡的請求資訊
        /// </summary>
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            // EnableBuffering 是允許 request 可以被多次讀取
            request.EnableBuffering();
            // 因為 stream 讀取玩進度就會停在 stream 的結尾 , 所以要先記錄下來待會再把它 "倒帶" 回去
            var body = request.Body;
            // 根據請求的內容長度預留一個型態為 byte 的空間
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            // 叫系統把剛剛的內容塞到 buffer 裡 , 待會要讀取 (buffer,開始讀取位置,結束位置)
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            // 轉換成字串 , 也就是我們能看得懂的
            string requestbody = Encoding.UTF8.GetString(buffer);
            // 將位置調回最開始 0
            body.Seek(0, SeekOrigin.Begin);
            // 傳回剛剛解讀出來得資訊
            return $"{requestbody}";
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{responseBody}";
        }
    }
}
