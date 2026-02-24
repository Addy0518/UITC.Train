


// 繼承父類別,可以使用本身子類別的物件也可以使用父類別的

Car car1 =new Car();
Boat boat1=new Boat();

Console.WriteLine(car1.speed);
Console.WriteLine(car1.color);
car1.Go();

Console.WriteLine(boat1.speed);
Console.WriteLine(boat1.weight);


// 子類別
class Car : Speed
{
    public string color = "black";

}
// 子類別
class Boat : Speed
{
    public int weight = 10;
}

// 父類別
class Speed
{
    public int speed = 20;

    public void Go()
    {
        Console.WriteLine("GOGOGO");
    }

}