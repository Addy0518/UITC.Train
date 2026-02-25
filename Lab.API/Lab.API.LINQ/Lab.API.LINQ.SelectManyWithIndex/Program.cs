using Dumpify;

IEnumerable<List<int>> collection = [[1, 2, 3], [4, 5, 6]];

// 用索引指定哪一個集合
collection.SelectMany((x,i) => x.Select(x => $"{i},{x}".ToString())).Dump();