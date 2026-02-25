using Dumpify;

IEnumerable<int> coll = [3, 1, 2];

coll.OrderBy(x=>x*2).Dump();