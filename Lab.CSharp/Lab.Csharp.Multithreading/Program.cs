

// 執行緒:可以設定時間來決定他執行的順序等等
Thread main = Thread.CurrentThread;
main.Name = "Main Thread";
//Console.WriteLine(main.Name);

Thread thread1 = new Thread(() => CountUp());
Thread thread2 = new Thread(() => CountDown());
thread1.Start();
thread2.Start();


//CountDown();
//CountUp();

//Console.WriteLine($"{main.Name}完成!");

static void CountUp()
{
    for (int i=1;i<=10;i++)
    {
        Console.WriteLine($"第二個執行緒 => 第{i}秒");
        Thread.Sleep(1000);
    }
    Console.WriteLine("第二個完成");

}

static void CountDown()
{
    for (int i = 10; i > 0; i--)
    {
        Console.WriteLine($"第一個執行緒 => 第{i}秒");
        Thread.Sleep(1000);
    }
    Console.WriteLine("第一個完成");

}