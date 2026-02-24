double temperature = 21;

// 這是if..else的寫法
if (temperature >= 25)
{
    Console.WriteLine("外面很熱!");
}
else
{
    Console.WriteLine("外面很冷!");
}

// 這是另一種寫法=>三元運算子, 設定條件?有達成:沒達成
Console.WriteLine((temperature >= 25) ? "外面很熱!" : "外面很冷!");