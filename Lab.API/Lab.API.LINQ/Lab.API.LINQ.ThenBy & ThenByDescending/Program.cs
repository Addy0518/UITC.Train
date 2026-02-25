using Dumpify;

IEnumerable<Person> coll = [new(0, "aa", 18), new(1, "bb", 19)];

coll.OrderBy(x=>x.Name).ThenBy(x=>x.age).Dump();

record Person(int Id, string Name, int age);