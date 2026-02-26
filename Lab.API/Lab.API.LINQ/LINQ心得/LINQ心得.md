# LINQ 的學習心得

## 撇除基本的語法以外,我覺得有一些語法在之前寫 API 的時候比較少遇到,所以這次練習也幫助我在回憶起來

## 下面幾個是我覺得比較困難或是重要的部分

---

### **1. Chunk** : Chunk本身不難,只是之前在寫的時候很少用到,所以剛開始聽到有點陌生,不過主要功能就是負責分組

### **2. Deferred Execution vs Immediate Execution** : 立即執行跟延遲執行,這是 LINQ 比較重要的部分,會依照使用方式能夠建造更複雜的查詢,

### 比如 select , where 這些延遲執行堆起來,再用toList()執行

### **3. Aggregate** : 這個我覺得比較難,他概念是把一個陣列的元素加總 第一個是初始值,在來往下加總

```csharp
 IEnumerable<int> collection = [1, 2, 3, 4, 5];

Console.WriteLine(collection.Aggregate(10,(x, y) =>x+y));
```

### **4. UnionBy & IntersectBy & Except** : 這三個的觀念很重要, union 代表全部合起來,但是重複的不顯示, IntersectBy 代表條件對上的,而 Except 則代表未出現在第 2 個序列的元素

```csharp
List<Person> coll = [new(1, "A", 15), new(2, "B", 16)];
List<Person> coll2 = [new(1, "A", 15)];

coll.UnionBy(coll2, x => x.Id).Dump();
coll.IntersectBy(coll2.Select(x=>x.Id), x => x.Id).Dump();
coll.ExceptBy(coll2.Select(x => x.Id), x => x.Id).Dump();

record Person(int Id, string Name, int age);
```

### **5. Join 跟 GroupJoin ** : Join 集合兩個 IEnumerable , GroupJoin 則是幫助彙總

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

### **6. FirstOrDefault** : 比較常用到 , 在沒有結果的情況下能夠給預設值讓程式不會報錯中斷

```csharp

IEnumerable<int> collection = [];


Console.WriteLine(collection.FirstOrDefault(-1));
```

### 7 . All , Any

1. All是要求所有查詢物件都要達到標準
2. Any則是有任一物件達到標準即可

```csharp
IEnumerable<int> collection = [1, 2, 3, 4, 5];
// 所有都要大於0
collection.All(x => x > 0).Dump();


IEnumerable<int> collection = [1, 2, 3, 4, 5];
// 其中一個大於2
collection.Any(x => x > 2).Dump();

```

### 8 . Distinct , DistinctBy

1. Distinct 是單純消除重複元素
2. DistinctBy 則是針對 " 屬性 " 去除重複

```csharp

IEnumerable<int> coll = [1, 2, 2, 2];
// 單純比對元素
coll.Distinct().Dump();


List<Person> coll = [
      new(1,"A",15),
      new(2,"B",16),
      new(2,"C",17),

    ];
// 針對某一個屬性去除重複
coll.DistinctBy(x => x.Id).Dump();
record Person(int Id,string Name, int age);
```

### 9 . Select , SelectMany , SelectManyWithIndex

1. Select 算是 最常用到的語法 , 用於尋找資料也可以指定元素
2. SelectMany 則會將select的結果扁平化,用來搜尋陣列或集合
3. 再加上Index的話就可以尋找索引

```csharp

IEnumerable<int> collection = [1, 2, 3, 4, 5];

// Select 的 Landa 函式裡,分別可以指定索引跟元素
collection.Select((x,i) => $"{i},{x}").Dump();


// SelectMany 可以展開多個陣列或集合,然後再用select選擇裡面的元素
collection.SelectMany(x => x.Select(x=>x.ToString())).Dump();

// 用索引指定哪一個集合
collection.SelectMany((x,i) => x.Select(x => $"{i},{x}".ToString())).Dump();

```

## 教學影片推薦的 Dumpify 套件不錯用 , 能夠讓結果的畫面更明瞭一點

##
