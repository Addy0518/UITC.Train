using Dumpify;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

collection.Any(x => x > 2).Dump();
