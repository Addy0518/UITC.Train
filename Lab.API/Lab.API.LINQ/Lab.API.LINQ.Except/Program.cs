using Dumpify;
IEnumerable<int> coll = [1, 2, 3];
IEnumerable<int> coll2 = [2, 3, 4];


coll.Except(coll2).Dump();