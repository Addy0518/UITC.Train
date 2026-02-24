


Dog dog = new Dog();
Cat cat=new Cat();
Boat boat=new Boat();

// 多形=>因為他們都繼承Vehicle,所以當我實體化一個類別為Vehicle的陣列,這三個物件就都可以放進來
Vehicle[] all = { dog, cat, boat };

foreach (Vehicle v in all)
{
    v.Go();
}

class Vehicle
{
    public virtual void Go()
    { 
    }
}
class Dog : Vehicle
{
    public override void Go()
    {
        Console.WriteLine("woo");
    }

}
class Cat : Vehicle
{
    public override void Go()
    {
        Console.WriteLine("mewo");
    }
}
class Boat : Vehicle
{
    public override void Go()
    {
        Console.WriteLine("fuck");
    }

}