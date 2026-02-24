
// 物件導向(OOP)
// 實例化新類別
Human h = new Human();
Human h2 = new Human();

// 填入變數的值
h.name = "陳";
h.age = 22;
h2.name = "王";

// 使用方法
h.Name();
h.Age();
h2.Name();

class Human
{
    public string name;
    public int age;
    public void Name()
    {
        Console.WriteLine($"My Name is {name}");
    }

    public void Age()
    {
        Console.WriteLine($"My Age Is {age}");
    }

}