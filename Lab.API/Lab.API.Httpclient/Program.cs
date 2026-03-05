using Lab.API.Httpclient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 在 Program 注入 Handler
// 補充 : AddTransient 跟 AddScope 的差別是 , AddScope 在一次請求中止會產生一個實例
// 而 AddTransient 則會在每次注入都產生實例 , 更小更輕量化
builder.Services.AddTransient<AppendQueryInUrlHandler>();

// 使用 httpclient 服務
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

// 統一管理相同基底的連線
builder
    .Services.AddHttpClient<JokeService>(x =>
    {
        x.BaseAddress = new Uri("http://official-joke-api.appspot.com/");
    })
    .AddHttpMessageHandler<AppendQueryInUrlHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
