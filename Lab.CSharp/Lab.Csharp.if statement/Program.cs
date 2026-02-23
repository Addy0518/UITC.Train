//Console.WriteLine("輸入年齡 :　");
//int age= Convert.ToInt32(Console.ReadLine());

//if (age <= 18)
//{
//    Console.WriteLine("必須年滿18!");
//}
//else if (age < 0)
//{
//    Console.WriteLine("你還沒出生!");
//}
//else
//{
//    Console.WriteLine("ok!");
//}


Console.WriteLine("輸入姓名 :　");
string name = Console.ReadLine();

if (name != "" && name != null)
{
    Console.WriteLine($"Hello , {name}");

}
else
{
    Console.WriteLine("沒輸入阿喂!");
}
