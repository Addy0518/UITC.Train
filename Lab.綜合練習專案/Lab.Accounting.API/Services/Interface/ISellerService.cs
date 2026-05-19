namespace Lab.Accounting.API.Services
{
    public interface ISellerService
    {
        /// <summary>
        /// 賣家註冊
        /// </summary>
        /// <param name="request">註冊資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> SellerRegister(SellerRegisterRequest request);
    }
}
