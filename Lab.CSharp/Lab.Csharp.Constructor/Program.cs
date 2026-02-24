


using System.Drawing;

Car c1 = new Car("黑色",350000);

c1.YourCar();



class Car
{
    string color;
    decimal price;


    public Car(string color,decimal price)
    { 
       this.color = color;
       this.price = price;
    }

    public void YourCar()
    {
        Console.WriteLine($"Your Car Is {color},And The Price Is {price}$");
    }

}