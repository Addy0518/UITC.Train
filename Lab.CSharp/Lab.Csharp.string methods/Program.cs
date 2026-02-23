using System.Globalization;

string fullname = "code";
string phonenumber = "123-456-7890";
fullname=fullname.ToUpper();//轉大寫
fullname=fullname.ToLower();//轉小寫

phonenumber = phonenumber.Replace("-", "=");//替換字元
string name = fullname.Insert(0, "@");//插入字元

string sub = fullname.Substring(0, 2);//擷取字元

Console.WriteLine(fullname.Length);//取得字串長度

Console.WriteLine(fullname);
Console.WriteLine(phonenumber);
Console.WriteLine(name);
Console.WriteLine(sub);