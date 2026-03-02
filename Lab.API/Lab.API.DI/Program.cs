using Lab.API.DI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 註冊 Interface , 實作 class
builder.Services.AddScoped<ISampleScoped, Sample>(); // Scope : 每個請求就 new 一個實例,也是比較常使用的
builder.Services.AddTransient<ISampleTransient, Sample>(); // Transient : 每次注入就 new 一個實例
builder.Services.AddSingleton<ISampleSingleton, Sample>(); // Singleton : 在整個程式運行期間只會有一個實例

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
