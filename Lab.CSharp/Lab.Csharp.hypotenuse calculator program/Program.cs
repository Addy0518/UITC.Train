Console.WriteLine("輸入A邊 :");
double a = Convert.ToDouble(Console.ReadLine());

Console.WriteLine("輸入B邊 :");
double b = Convert.ToDouble(Console.ReadLine());

double c = Math.Sqrt((a * a) + (b * b));

Console.WriteLine($"斜邊是 : {c}");