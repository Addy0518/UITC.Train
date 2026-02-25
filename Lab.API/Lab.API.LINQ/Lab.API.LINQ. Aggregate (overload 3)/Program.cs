

IEnumerable<int> collection = [1, 2, 3, 4, 5,6];


Console.WriteLine(collection.Aggregate(0,(x, y) => x + y,x=>(float)x/collection.Count()));
