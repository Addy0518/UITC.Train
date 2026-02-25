using Dumpify;

IEnumerable<object> collection = [1, "2", 3, 4, 5];

// 從末端作為起點
collection.SkipLast(3).Dump();
collection.TakeLast(3).Dump();