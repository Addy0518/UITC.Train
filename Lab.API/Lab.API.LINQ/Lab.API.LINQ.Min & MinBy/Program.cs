

IEnumerable<int> collection = [1, 2, 3, 4, 5];


Console.WriteLine(collection.MinBy(x => x * -1));
