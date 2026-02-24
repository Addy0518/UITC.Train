

Dog dog1=new Dog();
Cat cat1=new Cat();


dog1.Speak();
cat1.Speak(); 


class Animal
{
    public virtual void Speak()
    {
        Console.WriteLine("brrrr");
    }

}

class Dog : Animal
{
    //覆蓋父類別方法 override=>virtual
    public override void Speak()
    {
        Console.WriteLine("woooo");
    }
}
class Cat : Animal
{
    public override void Speak()
    {
        Console.WriteLine("meowwww");
    }

}