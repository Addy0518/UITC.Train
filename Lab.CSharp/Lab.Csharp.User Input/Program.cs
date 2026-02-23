Console.WriteLine("What Your Name?");

string username=Console.ReadLine();//從鍵盤輸入資料


Console.WriteLine("What Youe age?");
int age=Convert.ToInt32(Console.ReadLine());//轉型成int

Console.WriteLine($"Hello,{username}");
Console.WriteLine($"Your age is {age}");