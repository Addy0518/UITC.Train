




// 抽象類別=>無法實例化,增加安全性
// 可以被繼承的子類別覆蓋
//Speed sp = new Speed();

Car car1=new Car();
car1.Go();
public class Car : Speed
{
    public override void Go()
    {
        Console.WriteLine("GOGO");
    }
}

public abstract class Speed
{
    public int speed = 20;

    public abstract void Go();
   

}
