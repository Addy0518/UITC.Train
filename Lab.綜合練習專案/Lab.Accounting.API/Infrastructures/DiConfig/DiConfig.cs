using Lab.Accounting.API.Common.Helpers;

namespace Lab.Accounting.API.Infrastructures.DiConfig
{
    public static class DiConfig
    {
        //管理註冊
        public static void AddDiConfig(this IServiceCollection services)
        {
            services.AddSingleton<TokenHelper>();

            services.AddSingleton<DBConnecting>();

            services.AddScoped<ILedgerRepositories, LedgerRepositories>();

            services.AddScoped<IUserRepositories, UserRepositories>();

            services.AddScoped<ILedgerItemCategoryRepositories, LedgerItemCategoryRepositories>();

            services.AddScoped<ILedgerService, LedgerService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<PasswordSecureHelper>();
        }
    }
}
