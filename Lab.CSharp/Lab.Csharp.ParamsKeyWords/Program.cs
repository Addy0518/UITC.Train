
Console.WriteLine(checkOut(2,3.99,1.4,100));


static double checkOut(params double[] prices)//params關鍵字,能讓一個方法接受多個可自由變化的變數
{
    double total = 0;
    foreach (double price in prices)//遍歷所有價格並加總
    {

        total += price;

    }
    return total;

}