int[] arr = { 1, 2, 3 };
double[] arr2 = { 1.0, 2.0, 3.0 };
string[] arr3 = { "A", "B", "C" };

DisplayEments(arr);
DisplayEments(arr2);
DisplayEments(arr3);

// 泛型:通用所有類別,可以$在要接收相同參數但是不同型態時
static void DisplayEments<T>(T[] array)
{
    foreach (T item in array)
    {
        Console.Write(item+" ");
    }
    Console.WriteLine();
}