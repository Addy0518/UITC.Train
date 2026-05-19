using Microsoft.AspNetCore.Identity.Data;

namespace Lab.Accounting.API.Services
{
    public class SellerService(ISellerRepository sellerRepository, IUserRepository userRepository) : ISellerService
    {
        /// <summary>
        /// 賣家註冊
        /// </summary>
        /// <param name="request">註冊資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> SellerRegister(SellerRegisterRequest request)
        {
            var exist = await sellerRepository.GetSeller(request.UserId);

            if (exist != null)
            {
                var errors = new Dictionary<string, string[]> { { "Seller", new[] { "該帳號已是賣家!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var seller = new Seller
                {
                    UserId = request.UserId,
                    SellerCompanyName = request.SellerCompanyName,
                    SellerUnifiedNumber = request.SellerUnifiedNumber,
                    SellerName = request.SellerName,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                };
                var result = await sellerRepository.SellerRegister(seller);

                if (result == null)
                    return ApiResponseHelper.InternalException<int>();

                var role = await userRepository.UpdateRole(request.UserId, RolesAuth.賣家);

                trxScope.Complete();
                return ApiResponseHelper.Success<int>(result, "成功!");
            }
        }
    }
}
