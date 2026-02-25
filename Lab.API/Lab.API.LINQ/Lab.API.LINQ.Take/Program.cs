using Dumpify;

IEnumerable<object> collection = [1, "2", 3, 4, 5];

//Take:從0開始選幾個
collection.Take(2).Dump();