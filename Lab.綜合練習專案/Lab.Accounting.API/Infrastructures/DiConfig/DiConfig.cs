namespace Lab.Accounting.API.Infrastructures.DiConfig
{
    public static class DiConfig
    {
        //管理註冊
        public static void AddDiConfig(this IServiceCollection services)
        {
            services.AddSingleton<DBConnecting>();

            services.AddScoped<ILedgerRepositories, LedgerRepositories>();

            services.AddScoped<ILedgerService, LedgerService>();

            services.AddScoped<ILedgerItemCategoryRepositories, LedgerItemCategoryRepositories>();
        }
    }
}
