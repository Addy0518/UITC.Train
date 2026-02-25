

using Dumpify;

IEnumerable<object> collection = [1, "2", 3, 4, 5];

// OfType: 規定型別  
IEnumerable<int> a= collection.OfType<int>().Dump();
IEnumerable<string> b = collection.OfType<string>().Dump();
