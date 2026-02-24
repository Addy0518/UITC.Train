

// 定義隨機值
Random random = new Random();

// 決定使用者要不要再玩一次
bool playAgain = true;
// 定義最小值和最大值
int min;
int max;
// 使用者猜的數字
int guess;
// 隨機產生的數字
int number;
// 猜的次數
int guesses;


// 當使用者想要再猜一次
while (playAgain)
{
    
    // 重新定義變數
    guess = 0;
    guesses = 0;
    min = 1;
    max = 100;
    Console.WriteLine($"歡迎來到猜數字遊戲!請猜一個{min}到{max}之間的數字!");

    // 產生隨機數
    number = random.Next(min, max + 1);

    while (guess != number)
    {
        guess=Convert.ToInt32(Console.ReadLine());
     
        if (guess < number)
        {
            // 最小值改成猜測的數字,縮小範圍
            min=guess;
            Console.WriteLine($"猜錯了,範圍在{min}跟{max}之間");
            
        }
        else if (guess > number)
        {
            // 最大值改成猜測的數字
            max = guess;
            Console.WriteLine($"猜錯了,範圍在{min}跟{max}之間");
           

        }
        else
        {
            Console.WriteLine($"恭喜猜對!答案是{guess}");
        
        }
        // 不管怎樣都會增加一次猜測數
        guesses++;
    }
    Console.WriteLine($"恭喜猜對!答案是{guess}");
    Console.WriteLine($"總共猜了{guesses}次");
    Console.Write("想要再玩一次嗎? (Y/N)");
    string playgame = Console.ReadLine();
    string result = playgame.ToUpper();
    if (result == "Y")
    {
        // y的話就再重來一次
        playAgain = true;
    }
    else
    {
        playAgain = false;
    }

}

