

IEnumerable<int> collection = [1, 2, 3, 4, 5];

// MAx找最大值,所有乘以-1那最大就是-1
Console.WriteLine(collection.Max(x=>x*-1));
