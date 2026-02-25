using Dumpify;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

//在開頭加上元素
collection.Append(5).Dump();
//在結尾加上元素
collection.Prepend(5).Dump();

