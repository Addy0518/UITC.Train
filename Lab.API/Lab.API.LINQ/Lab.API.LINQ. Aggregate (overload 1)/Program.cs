

IEnumerable<int> collection = [1, 2, 3, 4, 5];


Console.WriteLine(collection.Select(x=>x.ToString()).Aggregate((x,y)=>$"{x},{y}"));
