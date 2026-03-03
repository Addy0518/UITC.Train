//
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var defaultConnectionString = builder.Configuration.GetValue<string>(
    "ConnectionStrings:DefaultConnection"
);

// 用 Configure 註冊並且型別是剛剛創建的 StrongholdInfoOptions
builder.Services.Configure<StrongholdInfoOptions>(
    // 再用 GetSection 指定內容是在 appsetting 的 StrongholdInfo
    builder.Configuration.GetSection("StrongholdInfo")
);

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
