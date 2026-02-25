

using Dumpify;
IEnumerable<int> coll = [1, 2, 3];
IEnumerable<int> coll2 = [2, 3, 4];


coll.Union(coll2).Dump();
coll.Intersect(coll2).Dump();
coll.Except(coll2).Dump();




