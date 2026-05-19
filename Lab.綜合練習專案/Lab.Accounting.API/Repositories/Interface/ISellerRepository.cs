namespace Lab.Accounting.API.Repositories.Interface
{
    public interface ISellerRepository
    {
        /// <summary>
        /// 賣家註冊
        /// </summary>
        /// <param name="seller">註冊資訊</param>
        /// <returns>影響列數</returns>
        Task<int> SellerRegister(Seller seller);

        /// <summary>
        /// 取得賣家資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣家資訊</returns>
        Task<Seller> GetSeller(int sellerId);
    }
}
