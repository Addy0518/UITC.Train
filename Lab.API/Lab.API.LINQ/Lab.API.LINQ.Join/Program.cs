

using Dumpify;

IEnumerable<Person> coll = [new(0, "aa", 18), new(1, "bb", 19)];
IEnumerable<Product> coll1 = [new(0, "ccc"), new(1, "ddd")];

coll.Join(
    coll1,
    person => person.Id,
    product => product.ProductId,
    (person, product) => $"{person.Name} bought {product.Name}").Dump();


record Person(int Id, string Name, int age);
record Product(int ProductId, string Name);