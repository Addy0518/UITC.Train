

using Dumpify;

IEnumerable<Person> coll = [new(0, "aa", 18), new(1, "bb", 19)];


IGrouping<int,Person> group=coll.GroupBy(x => x.Id).Last().Dump();


foreach (var person in group)
{
    person.Dump();
  
}



record Person(int Id, string Name, int age);
