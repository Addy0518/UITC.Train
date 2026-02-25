using Dumpify;

IEnumerable<List<int>> collection = [[1,2,3],[4,5,6]];

// SelectMany可以展開多個陣列或集合,然後再用select選擇裡面的元素
collection.SelectMany(x => x.Select(x=>x.ToString())).Dump();