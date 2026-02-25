


Car car = new Car("Tesla");

Console.WriteLine(car.Model);


class Car
{
    // 如果沒有特別要設定條件的話,就可以簡寫成像這樣,這就是自動屬性
    public string Model { get; set; }

    
    public Car(string model)
    { 
       this.Model = model;
    }

}