using Dumpify;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

// Select的Landa函式裡,分別可以指定索引跟元素
collection.Select((x,i) => $"{i},{x}").Dump();