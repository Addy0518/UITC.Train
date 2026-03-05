using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.Httpclient.Controllers
{
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
}
