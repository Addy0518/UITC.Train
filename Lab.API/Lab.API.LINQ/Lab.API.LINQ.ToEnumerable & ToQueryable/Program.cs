

List<Person> coll = [
      new("A",15),
      new("B",16),
      new("C",17),

    ];


IQueryable<Person> foo=coll.AsQueryable();
IEnumerable<Person> fooo=coll.AsEnumerable();

record Person(string Name, int age);




