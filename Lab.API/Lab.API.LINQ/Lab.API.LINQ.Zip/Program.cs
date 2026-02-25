using Dumpify;

IEnumerable<int> coll = [1, 2, 3];
IEnumerable<string> coll1 = ["A", "b"];
IEnumerable<string> coll2 = ["xx", "xxx"];

coll.Zip(coll1, coll2).Dump();