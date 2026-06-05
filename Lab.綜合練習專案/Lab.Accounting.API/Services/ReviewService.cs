using Lab.Accounting.API.Common.Requests.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using NPOI.HPSF;
using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Services
{
    public class ReviewService(
        IProductsReviewRepository productsReviewRepository,
        IProductsRepository productsRepository,
        IProductsImgRepository productsImgRepository,
        IWebHostEnvironment env
    ) : IReviewService
    {
        /// <summary>
        /// 查看商品審核
        /// </summary>
        /// <param name="reviewId">審核表 ID </param>
        /// <returns>審核資訊</returns>
        public async Task<ApiResponse<Review>> GetProductsReview(int reviewId)
        {
            var result = await productsReviewRepository.GetProductsReview(reviewId);

            if (result == null)
            {
                return ApiResponseHelper.NotFound<Review>();
            }

            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 查看所有商品審核
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>審核資訊</returns>
        public async Task<ApiResponse<ReviewResponse>> GetAllProductsReview(ProductsRiviewSearchRequest request)
        {
            var target = await productsReviewRepository.GetAllProductsReview(request);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<ReviewResponse>();
            }

            var response = new ReviewResponse
            {
                ProductsReview = target,
                TotalCount = target.FirstOrDefault()?.TotalCount ?? 0,
            };

            return ApiResponseHelper.Success(response);
        }

        /// <summary>
        /// 審核通過或駁回
        /// </summary>
        /// <param name="request">商品審核請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> ApproveOrRejectProductsReview(ProductsRivewRequest request)
        {
            if (request.ReviewStatus == ReviewStatusEnum.Reject && request.NotPassReson == null)
            {
                var errors = new Dictionary<string, string[]> { { "NotPassReson", new[] { "請填寫駁回原因 !" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsReviewRepository.ApproveOrRejectProductsReview(request);

                if (target <= 0)
                    return ApiResponseHelper.InternalException<int>("申請審核失敗");

                // 通過申請
                if (request.ReviewStatus == ReviewStatusEnum.Approved)
                {
                    var reviewInfo = await productsReviewRepository.GetProductsReview(request.ProductsReviewId);
                    // 判斷是更新商品還是新增商品 ( Id 是 null 就是新增 )
                    if (reviewInfo.ProductsId == null)
                    {
                        var createInfo = new MallProducts
                        {
                            UserId = reviewInfo.SellerId,
                            ProductsName = reviewInfo.ProductsName,
                            ProductsPrice = reviewInfo.ProductsPrice,
                            ProductsStock = reviewInfo.ProductsStock,
                            ProductsDescription = reviewInfo.ProductsDescription,
                            ProductCategoryId = reviewInfo.ProductCategoryId,
                            ReviewStatus = reviewInfo.ReviewStatus,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            IsDelete = IsDeleteStatusEnum.Normal,
                        };
                        var Insert = await productsRepository.CreateProducts(createInfo);
                        if (Insert <= 0)
                            return ApiResponseHelper.InternalException<int>("商品新增失敗");
                        await productsImgRepository.UpdateImgsToProductId(reviewInfo.ProductsReviewId, Insert);
                        await productsRepository.UpdateReviewProductsId(reviewInfo.ProductsReviewId, Insert);
                    }
                    else if (request.ProductsId != null)
                    {
                        var updateInfo = new MallProducts
                        {
                            ProductsId = reviewInfo.ProductsId ?? request.ProductsId,
                            UserId = reviewInfo.SellerId,
                            ProductsName = reviewInfo.ProductsName,
                            ProductsPrice = reviewInfo.ProductsPrice,
                            ProductsStock = reviewInfo.ProductsStock,
                            ProductsDescription = reviewInfo.ProductsDescription,
                            ProductCategoryId = reviewInfo.ProductCategoryId,
                            ReviewStatus = reviewInfo.ReviewStatus,
                            UpdateTime = DateTime.Now,
                        };
                        var update = await productsRepository.UpdateProducts(updateInfo);
                        if (update <= 0)
                            return ApiResponseHelper.InternalException<int>("商品更新失敗");
                        await productsImgRepository.UpdateImgsToProductId(
                            reviewInfo.ProductsReviewId,
                            reviewInfo.ProductsId.Value
                        );
                    }
                }
                // 駁回申請
                if (request.ReviewStatus == ReviewStatusEnum.Reject)
                {
                    // 查出這筆審核的所有圖片
                    var imgs = await productsImgRepository.GetReviewAllImg(request.ProductsReviewId);
                    // 全部刪除
                    foreach (var img in imgs)
                    {
                        FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", img.ProductsImg);

                        await productsImgRepository.DeleteProductsImg(img.ProductsImgId);
                    }
                }
                trxScope.Complete();
                return ApiResponseHelper.Success(target);
            }
        }
    }
}
