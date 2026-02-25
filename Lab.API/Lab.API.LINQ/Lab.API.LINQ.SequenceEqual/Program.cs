using Dumpify;

IEnumerable<int> coll = [1, 2, 3];
IEnumerable<int> coll1 = [1, 2, 3];
IEnumerable<int> coll2 = [1, 2, 3,4];

coll.SequenceEqual(coll2).Dump();