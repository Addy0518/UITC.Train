
double a;
double b;
double result;

Console.Write("輸入數字a : ");
a=Convert.ToDouble(Console.ReadLine());

Console.Write("輸入數字b : ");
b = Convert.ToDouble(Console.ReadLine());

Console.Write($"計算結果 : {Muiltpy(a,b)}");



static double Muiltpy(double x,double y)//方法可以回傳值,設定回傳值的型態並return
{

    return x * y;
 
}