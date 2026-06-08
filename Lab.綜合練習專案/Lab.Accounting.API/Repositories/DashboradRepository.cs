using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public class DashboradRepository(DBConnecting connecting) : IDashboradRepository
    {
        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="sellerId">賣家 ID</param>
        /// <returns>影響列數</returns>
        //public async Task<double> GetTotalRevenue()
        //{
        //    using var conn = connecting.CreateConnecting();

        //    var sql =
        //        @"INSERT INTO [dbo].[seller]
        //                    (userid,
        //                     sellername,
        //                     sellerunifiednumber,
        //                     sellercompanyname,
        //                     createtime,
        //                     updatetime,
        //                     IsDelete)
        //        VALUES      (@userid,
        //                     @SellerName,
        //                     @SellerUnifiedNumber,
        //                     @SellerCompanyName,
        //                     GetDate(),
        //                     GetDate(),
        //                     @IsDelete)

        //        SELECT Cast(@@ROWCOUNT AS INT) ";

        //    return await conn.ExecuteAsync(sql, seller);
        //}
    }
}
