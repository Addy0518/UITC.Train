


//Console.WriteLine((int)Car.Benz+" "+Car.Honda);

// 可以拿取它的名稱,或是值
string name = CarPrice.Honda.ToString();
int price = (int)CarPrice.Honda;
double count = Count(CarPrice.Tesla);

Console.WriteLine($"CarName={name} , CarPrice={price},Count={count}");


static double Count(CarPrice car)
{
    double Price = (int)car * 0.75;
    return Price;
}


// 列舉:可以自己設定一套命名標準,名稱會依預設排序(0=>1=>2...),也可以自訂他的值
enum CarPrice
{
  Tesla=300,
  Honda=500,
  Benz=600


}