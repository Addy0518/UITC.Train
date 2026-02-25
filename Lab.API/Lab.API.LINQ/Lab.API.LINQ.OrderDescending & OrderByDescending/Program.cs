using Dumpify;

IEnumerable<int> coll = [3, 1, 2];

coll.OrderDescending().Dump();
coll.OrderByDescending(x => x * 2).Dump();