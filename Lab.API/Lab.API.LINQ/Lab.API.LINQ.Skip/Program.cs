using Dumpify;

IEnumerable<object> collection = [1, "2", 3, 4, 5];

//Skip跳過
collection.Skip(3).Dump();