using System.Globalization;

Console.WriteLine("What day is today?");
string day = Console.ReadLine();

switch (day)//switch case語法
{
   case "Monday":
        Console.WriteLine("星期一"); break;

   case"Tuesday":
        Console.WriteLine("星期二"); break;

   default: Console.WriteLine("要輸入星期幾!");break;
}