

Cars c1 = new Cars();
Cars c2 = new Cars();

// 我實例了兩次所以會是2
Console.WriteLine(Cars.carsNumbers);

Cars.StartRun();

class Cars
{
    // static靜態方法,指向同一個記憶體所以會共用一個空間
    public static int carsNumbers;

    public Cars()
    {
        carsNumbers++;
    }

    public static void StartRun()
    {
        Console.WriteLine("開始比賽!");
    }
}