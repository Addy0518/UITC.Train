namespace Lab.Accounting.API.Infrastructures.DiConfig
{
    public static class DiConfig
    {
        //管理註冊
        public static void AddDiConfig(this IServiceCollection services)
        {
            services.AddSingleton<AccountConne>();

            services.AddScoped<IAccountRepositories, AccountRepositories>();

            services.AddScoped<IAccountService, AccountService>();
        }
    }
}
