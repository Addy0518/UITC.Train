

// class類別,用來宣告物件概念,是物件導向的基礎
int result = Messages.Counting([5, 3]);
Console.WriteLine(result);

Messages.Helo();

class Messages
{
    // 實作方法,public是公開,static靜態方法,void指沒有回傳值
    public static void Helo()
    {
        Console.WriteLine("Hello!");
    
    }

    public static int Counting(int[] numbers)
    {
        return numbers.Count();
    }

}


