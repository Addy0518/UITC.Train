using Dumpify;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

collection.All(x => x > 0).Dump();
