using Dumpify;

IEnumerable<string> coll1 = ["A", "b"];
IEnumerable<string> coll2 = ["xx", "xxx"];

coll1.Concat(coll2).Dump();