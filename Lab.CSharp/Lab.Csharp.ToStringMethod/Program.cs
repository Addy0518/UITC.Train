
Car car1 = new Car("Tesla", "Black");


// 本來的型別string就被我自訂的ToString()覆蓋了
Console.WriteLine(car1);


class Car
{
    public string model;
    public string color;

    public Car(string model, string color)
    {
        this.model = model;
        this.color = color;
    }

    public override string ToString()
    {
        return $"This is A Car";
    }
}