

// 繼承=>可以定義方法給其他類別繼承,也比較安全

Car car=new Car();
car.Run();
Boat boat=new Boat();
boat.Run();
boat.Water();



interface IRun
{ 
   void Run();
}
interface IWater
{
    void Water();
}


class Car:IRun
{
    public void Run()
    {
        Console.WriteLine("Drive a Car");
    }
}
class Boat:IRun,IWater
{
    public void Run()
    {
        Console.WriteLine("Drive a Boat");
    }

    public void Water()
    {
        Console.WriteLine("Water");
    }

}