Random random= new Random();//創建隨機數

int number=random.Next(1, 101);//產生1-100的隨機數 
double doub=random.NextDouble();//產生介於0-1的隨機小數

Console.WriteLine(doub);