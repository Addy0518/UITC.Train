public class StrongholdInfoOptions
{
    /// <summary>
    /// 關隘編號
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 關隘名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 關隘啟用狀態
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 駐守人員
    /// </summary>
    public string[]? General { get; set; }
}
