Random random = new Random();
bool playAgain = true;
// 玩家出的拳
string player;
// 電腦出的拳
string computer;


while (playAgain)
{
    player = "";
    computer = "";

    // 防呆
    while (player != "剪刀"&& player!="石頭"&&player != "布")
    {
        Console.WriteLine("輸入剪刀,石頭,布");
        player=Console.ReadLine();
    }
    // 生成1~3的隨機數字代表剪刀石頭布
    switch (random.Next(1, 4))
    {
        case 1:
            computer = "剪刀";
            if (player == "剪刀")
            {
                Console.WriteLine("平手!");
            }
            else if (player == "石頭")
            {
                Console.WriteLine("你贏了!");

            }
            else
            {
                Console.WriteLine("你輸了..");
            }
            break;
        case 2:
            computer = "石頭";
            if (player == "石頭")
            {
                Console.WriteLine("平手!");
            }
            else if (player == "布")
            {
                Console.WriteLine("你贏了!");

            }
            else
            {
                Console.WriteLine("你輸了..");
            }
            break;
        case 3:
            computer = "布";
            if (player == "布")
            {
                Console.WriteLine("平手!");
            }
            else if (player == "剪刀")
            {
                Console.WriteLine("你贏了!");

            }
            else
            {
                Console.WriteLine("你輸了..");
            }
            break;
    }
    Console.WriteLine($"玩家出拳 : {player}");
    Console.WriteLine($"電腦出拳 : {computer}");
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