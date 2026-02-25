

using Dumpify;

IEnumerable<int> collection = [1, 2, 3];

collection.ToDictionary(key => key, value => value).Dump();
