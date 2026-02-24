List<String> Food = new List<String>();

Food.Add("pizza");
Food.Add("banana");
Food.Add("rice");

Food.Remove("pizza");// 去除
Food.Insert(0, "soup");// 插入指定位置
Console.WriteLine(Food.Count());// 數量
Console.WriteLine(Food.Contains("rice")); // 包含
Food.Sort();// 排列
