using Microsoft.Net.Http.Headers;

namespace Lab.API.Httpclient
{
    public class GithubServie
    {
        private readonly HttpClient _httpclient;

        public GithubServie(HttpClient httpclient)
        {
            _httpclient = httpclient;

            _httpclient.BaseAddress = new Uri("https://api.github.com/");

            _httpclient.DefaultRequestHeaders.Add(
                HeaderNames.Accept,
                "application/vnd.github.v3+json"
            );

            _httpclient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
        }
    }
}
