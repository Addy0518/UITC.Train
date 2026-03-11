using Lab.API.TODO.Infrastructures.Data;
using Lab.API.TODO.Repositories.Implements;
using Lab.API.TODO.Repositories.Interfaces;

namespace Lab.API.TODO.Infrastructures.DependencyInjection
{
    public static class DIConfig
    {
        // 在這裡統一管理註冊 DI
        public static void AddDIConfig(this IServiceCollection services)
        {
            services.AddSingleton<TestConnection>();

            services.AddScoped<ITestService, TestService>();

            services.AddScoped<ITestRepository, TestRepository>();
        }
    }
}
