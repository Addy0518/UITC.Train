namespace Lab.Accounting.API.Services;

public class ShoppingCarService(
    IProductsRepository productsRepositories,
    IProductsImgRepository productsImgRepository,
    IProductsShoppingCarRepository productsShoppingCarRepositories,
    IUserRepository userRepository
) : IShoppingCarService
{
    /// <summary>
    /// 查看購物車中的所有商品
    /// </summary>
    /// <param name="userId">使用者 Id</param>
    /// <returns>購物車中的所有商品</returns>
    public async Task<ApiResponse<IEnumerable<ProductDetails>>> GetAllProductsInShoppingCar(int userId)
    {
        var alltarget = await productsShoppingCarRepositories.GetAllProductsInShoppingCar(userId);
        if (alltarget == null || !alltarget.Any())
        {
            return ApiResponseHelper.NotFound<IEnumerable<ProductDetails>>();
        }
        foreach (var target in alltarget)
        {
            var imgs = await productsImgRepository.GetProductsAllImg(target.ProductsId);
            target.ProductsImgs = imgs;
        }
        return ApiResponseHelper.Success<IEnumerable<ProductDetails>>(alltarget);
    }

    /// <summary>
    /// 新增單一商品到購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <param name="boughtquantity">購買數量</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> AddProductsInShoppingCar(int productsId, int userId, int boughtquantity)
    {
        var user = await userRepository.GetUser(userId);
        var product = await productsRepositories.GetProducts(productsId);
        if (user.UserRole == RolesAuth.賣家 && product.UserId == userId)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "UserRole", new[] { "賣家無法將自己的商品加入購物車!" } },
            };

            return ApiResponseHelper.RequestError<int>(errors);
        }
        if (boughtquantity <= 0)
        {
            var errors = new Dictionary<string, string[]> { { "BoughtQuantity", new[] { "請輸入購買數量!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

        if (product == null)
            return ApiResponseHelper.NotFound<int>();

        var target = await productsShoppingCarRepositories.AddProductsInShoppingCar(productsId, userId, boughtquantity);
        if (target == 0)
            return ApiResponseHelper.InternalException<int>("加入購物車失敗，請稍後再試");
        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 刪除單一商品從購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> DeleteProductsInShoppingCar(int productsId, int userId)
    {
        var target = await productsShoppingCarRepositories.DeleteProductsInShoppingCar(productsId, userId);
        if (target == 0)
        {
            return ApiResponseHelper.NotFound<int>();
        }
        return ApiResponseHelper.Success(target);
    }
}
