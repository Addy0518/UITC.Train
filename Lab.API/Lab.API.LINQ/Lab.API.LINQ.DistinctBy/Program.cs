

using Dumpify;

List<Person> coll = [
      new(1,"A",15),
      new(2,"B",16),
      new(2,"C",17),

    ];

coll.DistinctBy(x => x.Id).Dump();
record Person(int Id,string Name, int age);




