global using Dumpy;
using Dumpy.Console;

IEnumerable<int> collection = [1, 2, 3, 4, 5];

// Where:設立條件
collection.Where(x=>x>2).Dump();