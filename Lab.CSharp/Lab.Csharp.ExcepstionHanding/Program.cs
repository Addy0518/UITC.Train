int x;
int y;

try // try..catch..finally,用來捕獲錯誤並防止程式中斷,finally是不管有沒有錯誤最後都會執行
{

    Console.Write("請輸入第一個數字 : ");
    x = Convert.ToInt32(Console.ReadLine());


    Console.Write("請輸入第二個數字 : ");
    y = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine(x / y);
}
catch (FormatException ex) // 格式錯誤
{
    Console.WriteLine("請輸入數字!");
}
catch (DivideByZeroException exx) // 整數除以0的錯誤
{
    Console.WriteLine("不能除以0!");
}
catch (Exception exxx) // 最終防線,用來抓所有錯誤的
{
    Console.WriteLine($"有錯誤!,{exxx.Message}");
}
finally
{
    Console.WriteLine("end");
}