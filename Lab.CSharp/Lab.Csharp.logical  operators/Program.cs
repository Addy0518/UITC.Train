Console.WriteLine("今天溫度多少?");
double temp=Convert.ToDouble(Console.ReadLine());

if (temp >= 10 && temp <= 25)//邏輯運算 &&,表示兩個條件都要成立
{
    Console.WriteLine("是個好天氣!");
}
else if (temp <= 0 || temp >= 50)//邏輯運算 ||,表示兩個條件只要有一個成立就好
{
    Console.WriteLine("什麼鬼天氣?");
}
else 
{
    Console.WriteLine("天氣比較差喔..");
}