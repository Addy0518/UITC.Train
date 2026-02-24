String[] cars = { "BMW", "BENZ", "HONDA" };//第一種宣告陣列的方式

String[] fruit = new String[3];//第2種

fruit[0] = "apple";
fruit[1] = "banana";
fruit[2] = "watermelon";



cars[0] = "TESLA";//更換陣列元素

//Console.WriteLine(cars[0]);

//用迴圈顯示陣列內所有元素
for (int i = 0; i < cars.Length; i++)
{
    Console.WriteLine(cars[i]);
}

for (int i = 0; i < fruit.Length; i++)
{
    Console.WriteLine(fruit[i]);
}