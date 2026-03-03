using Lab.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  https://localhost:xxxx/Map1
app.Map(
    "/map1",
    mapApp =>
    {
        // 順序是 in => Second => out
        mapApp.Use(
            async (context, next) =>
            {
                await context.Response.WriteAsync("Second Middleware in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Second Middleware out. \r\n");
            }
        );
        mapApp.Run(async context =>
        {
            await context.Response.WriteAsync("Second. \r\n");
        });
    }
);

app.Map("/map2", Map2);
app.Map("/map3", Map3);

// 巢狀結構 , 要先執行 Map2 在執行 nextMap2
static void Map2(IApplicationBuilder app)
{
    app.Map(
        "/nextMap2",
        mapApp =>
        {
            mapApp.Use(
                async (context, next) =>
                {
                    await context.Response.WriteAsync("nextMap2 IN. \r\n");
                    await next.Invoke();
                    await context.Response.WriteAsync("nextMap2 out. \r\n");
                }
            );
            mapApp.Run(async context =>
            {
                await context.Response.WriteAsync("Second. \r\n");
            });
        }
    );
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map 1");
    });
}
static void Map3(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map 2");
    });
}

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
