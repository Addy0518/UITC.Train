Console.Write("輸入姓名 :　");
string name = Console.ReadLine();

//while迴圈,沒達到條件就一直執行
while (String.IsNullOrWhiteSpace(name))
{
    Console.Write("輸入姓名 :　");
    name = Console.ReadLine();
}


Console.WriteLine($"Hello , {name}");