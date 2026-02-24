
// 多維陣列
String[,] cars = { {"HONDA","TOYOTA"}, {"BMW","BENZ" },{ "FORARII","TESLA"} };

Console.WriteLine(cars[0,1]);

// 把她網格化
// GetLength(0)拿到列,GetLength(1)拿到欄
for (int i = 0; i < cars.GetLength(0); i++)
{
    for (int j = 0; j < cars.GetLength(1); j++)
    {
        Console.Write(cars[i,j]+"\t ");// 排整齊
    }
    Console.WriteLine();
}