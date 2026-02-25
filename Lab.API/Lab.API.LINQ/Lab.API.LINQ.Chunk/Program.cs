using Dumpify;

IEnumerable<object> collection = [1, 2, 3, 4, 5];

// 分組
collection.Chunk(3).Dump();