using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.Httpclient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // 直接注入 IHttpClientFactory , 使用 HttpClient 的功能
    public class UniversitiesController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
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

        // 第二種
        [HttpGet("v2/{country}")]
        public async Task<IActionResult> GetListv2([FromRoute] string country)
        {
            // 建立連線
            var httpclient = httpClientFactory.CreateClient("universities");
            // 寄送請求
            var response = await httpclient.GetAsync($"search?country={country}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStreamAsync();
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("jokes")]
        public async Task<IActionResult> GetJokes()
        {
            // 建立連線
            var httpclient = httpClientFactory.CreateClient("jokes");
            // 改成用 Json 格式去讀取訊息
            var response = await httpclient.GetFromJsonAsync<JokeModel>("random_joke");

            return Ok(response);
        }
    }
}
