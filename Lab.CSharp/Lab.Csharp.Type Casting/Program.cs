double a = 3.14;
int b = Convert.ToInt32(a);//轉型

int c = 123;
double d= Convert.ToDouble(c)+0.1;

int e = 321;
string f=Convert.ToString(e);

string g = "$";
char h= Convert.ToChar(g);

string i = "true";
bool j=Convert.ToBoolean(i);

Console.WriteLine(a.GetType());//GetType()拿到變數型態
Console.WriteLine(d.GetType());
Console.WriteLine(f);
Console.WriteLine(h);
Console.WriteLine(j.GetType());