# LINQ的學習心得

## 撇除基本的語法以外,我覺得有一些語法在之前寫API的時候比較少遇到,所以這次練習也幫助我在回憶起來

## 下面幾個是我覺得比較困難或是重要的部分

---

### **1.Chunk** : Chunk本身不難,只是之前在寫的時候很少用到,所以剛開始聽到有點陌生,不過主要功能就是負責分組

### **2.Deferred Execution vs Immediate Execution** : 立即執行跟延遲執行,這是LINQ比較重要的部分,會依照使用方式能夠建造更複雜的查詢,

### 比如select,where這些延遲執行堆起來,再用toList()執行

### **3.Aggregate** : 這個我覺得比較難,他概念是把一個陣列的元素加總 第一個是初始值,在來往下加總

```csharp
 IEnumerable<int> collection = [1, 2, 3, 4, 5];

Console.WriteLine(collection.Aggregate(10,(x, y) =>x+y));
```

### **4.UnionBy & IntersectBy & Except** : 這三個的觀念很重要,union代表全部合起來,但是重複的不顯示,IntersectBy代表條件對上的,而Except則代表未出現在第2個序列的元素

```csharp
List<Person> coll = [new(1, "A", 15), new(2, "B", 16)];
List<Person> coll2 = [new(1, "A", 15)];

coll.UnionBy(coll2, x => x.Id).Dump();
coll.IntersectBy(coll2.Select(x=>x.Id), x => x.Id).Dump();
coll.ExceptBy(coll2.Select(x => x.Id), x => x.Id).Dump();

record Person(int Id, string Name, int age);
```

### **5.Join跟GroupJoin** : Join集合兩個IEnumerable,GroupJoin則是幫助彙總

```csharp
IEnumerable<Person> coll = [new(0, "aa", 18), new(1, "bb", 19)];
IEnumerable<Product> coll1 = [new(0, "ccc"), new(1, "ddd")];

coll.Join(
    coll1,
    person => person.Id,
    product => product.ProductId,
    (person, product) => $"{person.Name} bought {product.Name}").Dump();


coll.GroupJoin(
    coll1,
    person => person.Id,
    product => product.ProductId,
    (person, product) => $"{person.Name} bought {string.Join(',',product)}").Dump();


record Person(int Id, string Name, int age);
record Product(int ProductId, string Name);
```

### **6.FirstOrDefault** : 比較常用到,在沒有結果的情況下能夠給預設值讓程式不會報錯中斷

```csharp

IEnumerable<int> collection = [];


Console.WriteLine(collection.FirstOrDefault(-1));
```
