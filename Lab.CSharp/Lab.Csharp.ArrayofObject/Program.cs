


// 陣列實體化物件

Car[] carArray = { new Car("Tesla"), new Car("Honda") };

foreach (Car car in carArray)
{
    Console.WriteLine(car.model);

}


class Car
{
    public string model;

    public Car(string model)
    { 
      this.model = model;
    }


}