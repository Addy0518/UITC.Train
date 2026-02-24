
string name = "Andy";
int age = 23;

singHappyBirthday(name,age);

// static靜態方法 不用實體化呼叫
// 可以把一些重複會用到的邏輯寫進方法,重複使用
static void singHappyBirthday(string name,int age)// 可以讓方法有接口能接變數(注意:要使用相同型態的變數)
{
    Console.WriteLine("祝你生日快樂!");
    Console.WriteLine("祝你生日快樂!");
    Console.WriteLine($"{name},{age}歲快樂!");
}