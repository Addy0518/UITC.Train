using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public class DashBoradRepository(DBConnecting connecting) : IDashBoradRepository
    {
        /// <summary>
        /// 查看賣家總銷售額
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>總銷售額</returns>
        public async Task<double> GetTotalRevenue(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select Sum([AccountPrice]) as TotalRevenue 
                  From [MallOrder] 
                  Where SellerUserId=@SellerUserId";

            return await conn.ExecuteScalarAsync<double>(sql, new { SellerUserId = sellerUserId });
        }

        /// <summary>
        /// 查看賣家本月份銷售額
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>本月份銷售額</returns>
        public async Task<double> GetMonthlyRevenue(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @" Select Sum([AccountPrice]) as TotalRevenue 
                From [MallOrder] m 
                where m.SellerUserId=@SellerUserId

                -- DATEDIFF(MONTH, 起點, 終點) => 計算起點到終點之間的月數差距
                -- DATEADD(MONTH, 要加幾個月, 起點) => 從起點開始加上月數差距後的日期
                and BoughtTIme >= DATEADD(MONTH,DATEDIFF(MONTH,0,getdate()),0) 
                and BoughtTIme <  DATEADD(MONTH,DATEDIFF(MONTH,0,getdate())+1,0)";

            return await conn.ExecuteScalarAsync<double>(sql, new { SellerUserId = sellerUserId });
        }

        /// <summary>
        /// 查看賣家近七天的個別銷售額
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>近七天個別銷售額</returns>
        public async Task<IEnumerable<WeekSalesResponse>> GetWeekSales(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT Cast([boughttime] AS DATE) AS OrderDate,
                         Sum([accountprice])          AS DailyRevenue
                FROM   [mallorder] m
                WHERE  m.selleruserid = @SellerUserId
                       AND boughttime BETWEEN Dateadd(day, -7, Cast(Getdate() AS DATE)) AND
                                              Dateadd(day, 1, Cast(Getdate() AS DATE))
                GROUP  BY Cast(boughttime AS DATE)
                ORDER  BY orderdate ";

            return await conn.QueryAsync<WeekSalesResponse>(sql, new { SellerUserId = sellerUserId });
        }

        /// <summary>
        /// 查看賣家庫存少於五的商品
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>商品</returns>
        public async Task<IEnumerable<MallProducts>> GetLowStockProducts(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select * 
                From [MallProducts] 
                WHERE UserId = @SellerUserId
                AND ProductsStock < 5
                Order by [ProductsStock] asc";

            return await conn.QueryAsync<MallProducts>(sql, new { SellerUserId = sellerUserId });
        }

        /// <summary>
        /// 查看賣家銷量最好的前五的商品
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>商品</returns>
        public async Task<IEnumerable<TopSellingResponse>> GetTopSellingProducts(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT TOP 5 p.productsid,
                             p.productsname,
                             p.productsprice,
                             p.productsstock,
                             Sum(o.boughtquantity) AS TotalSales
                FROM   [mallproducts] p
                       JOIN [mallorder] o
                         ON p.productsid = o.productsid
                WHERE  p.userid = @SellerUserId
                       AND p.isdelete = 0
                GROUP  BY p.productsid,
                          p.productsname,
                          p.productsprice,
                          p.productsstock
                ORDER  BY totalsales DESC ";

            return await conn.QueryAsync<TopSellingResponse>(sql, new { SellerUserId = sellerUserId });
        }

        /// <summary>
        /// 查看賣家的所有商品評分分布
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>評分分布</returns>
        public async Task<IEnumerable<DashBoardRateResponse>> GetRateDistribution(int sellerUserId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select [Rating] as RateDistribution,Count(*) as RateCount 
                From [MallProductsRate] 
                Where ProductsId in (Select ProductsId from MallProducts where UserId=@SellerUserId)
                Group by Rating 
                Order by Rating ";

            return await conn.QueryAsync<DashBoardRateResponse>(sql, new { SellerUserId = sellerUserId });
        }
    }
}
