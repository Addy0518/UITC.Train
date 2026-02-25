

IEnumerable<int> collection = [1, 2, 3, 4, 5];


Console.WriteLine(collection.Aggregate(10,(x, y) =>x+y));
