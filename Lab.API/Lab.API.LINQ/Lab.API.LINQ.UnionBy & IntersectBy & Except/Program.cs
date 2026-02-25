

using Dumpify;

List<Person> coll = [new(1, "A", 15), new(2, "B", 16)];
List<Person> coll2 = [new(1, "A", 15)];

coll.UnionBy(coll2, x => x.Id).Dump();
coll.IntersectBy(coll2.Select(x=>x.Id), x => x.Id).Dump();
coll.ExceptBy(coll2.Select(x => x.Id), x => x.Id).Dump();

record Person(int Id, string Name, int age);




