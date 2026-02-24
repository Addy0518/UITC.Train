do
{
    int result = 0;
    Console.Write("這是計算機,請輸入第一個數字 :　");
    int num1 =Convert.ToInt32(Console.ReadLine());

    Console.Write("第二個數字 :　");
    int num2 = Convert.ToInt32(Console.ReadLine());

    Console.Write("計算方式?(+,-,*,/) : ");


    switch (Console.ReadLine())
    {
        case "+":
           result=num1 + num2;
            break;
        case "-":
            result=num1 - num2;
            break;
        case "*":
            result=num1 * num2;
            break;
        case "/":
            result=num1 / num2;
            break;
        default:
            Console.WriteLine("請輸入正確計算符號!");
            break;

            
    }
    Console.WriteLine($"計算結果是 : {result}");
    Console.Write("想要再算一次嗎? (Y/N)");
   
}
// 在最後有達成條件就在進入迴圈(do...while)
while (Console.ReadLine().ToUpper() == "Y");
