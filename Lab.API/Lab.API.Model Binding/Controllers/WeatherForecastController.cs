using Microsoft.AspNetCore.Mvc;
using static Lab.API.Model_Binding.Models.TodoItem;

namespace Lab.API.Model_Binding.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching",
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                })
                .ToArray();
        }

        // /WeatherForecast/range?daterange=2024-05-20,2024-05-30
        // 從 url 拿取參數
        [HttpGet("range")]
        public IActionResult ByRange([FromQuery] DateRange daterange)
        {
            var weatherForecasts = Enumerable
                .Range(1, 10)
                .Select(index => new
                {
                    Date = DateTime.Today.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                })
                .Select(wf => new { DateOnly = DateOnly.FromDateTime(wf.Date), Original = wf })
                .Where(x =>
                    (daterange.From == null || x.DateOnly >= daterange.From)
                    && (daterange.To == null || x.DateOnly <= daterange.To)
                )
                .Select(x => new
                {
                    Date = x.DateOnly.ToString("yyyy-MM-dd"),
                    x.Original.TemperatureC,
                    TemperatureF = 32 + (int)(x.Original.TemperatureC / 0.5556),
                    x.Original.Summary,
                })
                .ToList();

            return Ok(weatherForecasts);
        }
    }
}
