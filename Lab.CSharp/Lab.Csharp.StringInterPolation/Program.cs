string name = "Andy";
int age = 22;

// 字串插值,在引號前面加$可以在字串裡插值,而在變數後面能決定要空幾格,負數則是顛倒
Console.WriteLine($"Hello,{name}");
Console.WriteLine($"Your age is {age,-10}");