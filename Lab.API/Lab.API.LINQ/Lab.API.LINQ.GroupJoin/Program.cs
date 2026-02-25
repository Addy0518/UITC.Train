

using Dumpify;

IEnumerable<Person> coll = [new(0, "aa", 18), new(1, "bb", 19)];
IEnumerable<Product> coll1 = [new(0, "ccc"), new(0, "aaaac"), new(1, "ddd")];

coll.GroupJoin(
    coll1,
    person => person.Id,
    product => product.ProductId,
    (person, product) => $"{person.Name} bought {string.Join(',',product)}").Dump();




record Person(int Id, string Name, int age);
record Product(int ProductId, string Name);