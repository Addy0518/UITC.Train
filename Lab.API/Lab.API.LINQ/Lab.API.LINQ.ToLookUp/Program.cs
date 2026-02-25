
using Dumpify;

IEnumerable<Person> coll = [
      new("A",15),
      new("B",16),
      new("C",17),

    ];


coll.ToLookup(x => x.age).Dump();

record Person(string Name, int age);
    



