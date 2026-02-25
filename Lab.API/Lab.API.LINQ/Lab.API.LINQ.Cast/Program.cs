using Dumpify;

IEnumerable<object> collection = [1, 2, 3, 4, 5];

// 強行別轉換
collection.Cast<int>().Dump();