

Car car1 = new Car();



car1.Speed = 10000;

Console.WriteLine(car1.Speed);


class Car
{

    private int speed;

    // 屬性：set設定進來的值,也可以在這裡給他條件,再用get return回去
    public int Speed
    {
       get{ return speed; }
       set
       {
            if (value > 500)
            {
                speed = 500;
            }
            else 
            {
             this.speed = value; 
            }

           
        
        }
    }

}