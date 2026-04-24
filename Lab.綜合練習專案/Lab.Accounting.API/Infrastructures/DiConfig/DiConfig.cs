using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Repositories.Interface;
using Lab.Accounting.API.Services.Interface;

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

            services.AddScoped<IProductsRepositories, ProductsRepositories>();

            services.AddScoped<IProductsImgRepository, ProductsImgRepository>();

            services.AddScoped<IProductsRateRepositories, ProductsRateRepositories>();

            services.AddScoped<ITokenBlacklistRepositories, TokenBlacklistRepositories>();

            services.AddScoped<ILedgerItemCategoryRepositories, LedgerItemCategoryRepositories>();

            services.AddScoped<IProductsShoppingCarRepositories, ProductsShoppingCarRepositories>();

            services.AddScoped<ILedgerService, LedgerService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IMallService, MallService>();

            services.AddScoped<PasswordSecureHelper>();
        }
    }
}
