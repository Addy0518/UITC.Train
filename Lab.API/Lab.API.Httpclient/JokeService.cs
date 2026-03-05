namespace Lab.API.Httpclient
{
    public class JokeService(HttpClient httpClient)
    {
        // 把這個讀取訊息的方法統一坐在一個 Servie , 這裡的基底位址都一樣
        public async Task<JokeModel?> GetJokeAsync()
        {
            return await httpClient.GetFromJsonAsync<JokeModel>("random_joke");
        }
    }
}
