using Dumpify;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

// 設條件,跳過滿足條件的欄位
collection.SkipWhile(x=>x<2).Dump();
// 設條件,只拿滿足條件的欄位
collection.TakeWhile(x=>x<2).Dump();