namespace Lab.API.Dapper.Models;

using Lab.API.Dapper.Repository;

// 分離出來一個註冊連線跟 Service 的類別 , 在註冊到 Program
public static class AddDIConfig
{
    // 注入服務
    public static void ADDDIConfig(this IServiceCollection services)
    {
        // 使用剛剛設定的 User 連線
        services.AddSingleton<UserConnection>();
        // 註冊 Repo 介面
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
