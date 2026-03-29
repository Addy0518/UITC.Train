namespace Lab.Accounting.API.Infrastructures.Entities
{
    public class LedgerItemDTO:LedgerItem
    {
        /// <summary>
        /// 項目類別名稱
        /// </summary>
        public string CategoryName { get; set; }
    }
}
