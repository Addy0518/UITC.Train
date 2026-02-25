using Dumpify;

IEnumerable<object> collection = [1, 2, 3, 4, 5];

// toList()讓sql執行   
var result= collection.Chunk(3).ToList();

result.Dump();
