

Car car1 = new Car("Tesla","Black");

// 把新實體化的車子物件的顏色改成黃色
ColorChange(car1, "Yellow");

Console.WriteLine(car1.model+" "+car1.color);

// 再度傳入car物件,去改變車子顏色
static void ColorChange(Car car,string color)
{
    car.color = color;
}

// 複製物件並回傳(類別就是Car)
static Car Copy(Car car)
{ 
  return new Car(car.model,car.color);

}

// 再使用這個複製的方法,丟car1物件進去,就可以複製car1
Car car2 = Copy(car1);
Console.WriteLine(car2.model+" "+car2.color);



class Car
{
    public string model;
    public string color;

    public Car(string model, string color)
    {
        this.model = model;
        this.color = color;
    }


}