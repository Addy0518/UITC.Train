
// 先定義欄數跟列數的變數
int rows, cols;


Console.Write("有多少列? : ");
var result1=Console.ReadLine();

Console.Write("有多少欄? : ");
var result2 = Console.ReadLine();

// 用TryParse防呆
if (int.TryParse(result1, out rows) == false || int.TryParse(result2, out cols) == false)
{
    Console.WriteLine("請輸入數字!");
    Console.ReadKey();
    return;
}

Console.Write("要用什麼符號? : ");
string symbol = Console.ReadLine();

if (string.IsNullOrWhiteSpace(symbol))
{
    Console.WriteLine("請輸入符號!");
    return;

}


for (int i = 0; i < rows; i++)// 用外迴圈跟內迴圈印出列數跟欄數
{
    for (int y = 0; y < cols; y++)
    {
      Console.Write(symbol);
    }
      Console.WriteLine();// 在每一欄結束後換行
}
