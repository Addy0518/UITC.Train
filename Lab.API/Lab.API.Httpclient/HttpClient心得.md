# HttpClient 心得

### 我使用兩個網路上公開的 API 作為練習 => http://universities.hipolabs.com/search ( 查詢任一國家的所有大學資訊 ) , http://official-joke-api.appspot.com/random_joke ( 隨機生成笑話 )

1.  第一種方法我使用 HttpClient 連線 , 去呼叫並接收這個 api 的回傳結果

```csharp
// 第一種
[HttpGet("v1/{country}")]
public async Task<IActionResult> GetListv1([FromRoute] string country)
{
    // 新建一個 http 連線
    HttpClient client = new HttpClient();
    // 加入基底位址
    client.BaseAddress = new Uri("http://universities.hipolabs.com/");
    // 看要使用哪種動作就用什麼動詞呼叫端點
    var response = await client.GetAsync($"search?country={country}");
    // 如果回傳了成功的狀態碼
    if (response.IsSuccessStatusCode)
    {
        // 用串流方式 ( ReadAsStreamAsync ) 讀取回傳結果 , 降低耗能
        var result = await response.Content.ReadAsStreamAsync();
        return Ok(result);
    }

    return BadRequest();
}
```

2. 第二種方法是使用 httpClientFactory 服務 , 他負責管理 http 的連線

```csharp
// 使用 httpclient 服務
builder.Services.AddHttpClient();

// 直接注入 IHttpClientFactory , 使用 HttpClient 的功能
public class UniversitiesController(IHttpClientFactory httpClientFactory) : ControllerBase

// 第二種
[HttpGet("v2 /{country}")]
public async Task<IActionResult> GetListv2([FromRoute] string country)
{
    // 使用 http 方法的 get , 跟 url 待會用來寄送
    var httpresponse = new HttpRequestMessage(
        HttpMethod.Get,
        $"http://universities.hipolabs.com/search?country={country}"
    );

    // 建立連線
    var httpclient = httpClientFactory.CreateClient();
    // 寄送請求
    var response = await httpclient.SendAsync(httpresponse);
    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadAsStreamAsync();
        return Ok(result);
    }
    return BadRequest();
}
```

3. 也可以把基底直接註冊在 Program

```csharp
builder.Services.AddHttpClient(
    "universities",
    x =>
    {
        x.BaseAddress = new Uri("http://universities.hipolabs.com/");
    }
);

// 可建立多個服務
builder.Services.AddHttpClient(
    "jokes",
    x =>
    {
        x.BaseAddress = new Uri("http://official-joke-api.appspot.com/");
    }
);
```

4. 建立一個新的 Model 用來讀取回傳訊息

```csharp
public class JokeModel
{
    public int id { get; set; }

    public string type { get; set; }

    public string setup { get; set; }

    public string punchline { get; set; }
}

// 簡化
[HttpGet("jokes")]
public async Task<IActionResult> GetJokes()
{
    // 建立連線
    var httpclient = httpClientFactory.CreateClient("jokes");
    // 改成用 Json 格式去讀取訊息
    var response = await httpclient.GetFromJsonAsync<JokeModel>("random_joke");

    return Ok(response);
}
```

4. 最好是做一個 Service 統一管理一樣類型的連線

```csharp
// 統一管理相同基底的連線
builder.Services.AddHttpClient<JokeService>(
    x =>
    {
        x.BaseAddress = new Uri("http://official-joke-api.appspot.com/");
    }
);

// Service
 public class JokeService(HttpClient httpClient)
 {
     // 把這個讀取訊息的方法統一坐在一個 Servie , 這裡的基底位址都一樣
     public async Task<JokeModel?> GetJokeAsync()
     {
         return await httpClient.GetFromJsonAsync<JokeModel>("random_joke");
     }
 }

// Controller
[Route("api/[controller]")]
[ApiController]
public class JokesController(JokeService jokeService) : ControllerBase
{
    // 這樣就會最簡化寫法
    [HttpGet]
    public async Task<IActionResult> GetJokeAsync()
    {
        return Ok(await jokeService.GetJokeAsync());
    }
}
```

5. 也可以對請求做一些處理 , 比如攔截請求並加工

```csharp
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


// 在 Program 注入 Handler
// 補充 : AddTransient 跟 AddScope 的差別是 , AddScope 在一次請求中止會產生一個實例
// 而 AddTransient 則會在每次注入都產生實例 , 更小更輕量化
builder.Services.AddTransient<AppendQueryInUrlHandler>();

// 使用 , 這樣每次發出請求的 Query 就會被更改
builder
    .Services.AddHttpClient<JokeService>(x =>
    {
        x.BaseAddress = new Uri("http://official-joke-api.appspot.com/");
    })
    .AddHttpMessageHandler<AppendQueryInUrlHandler>();

```