



// 可以建立多個相同名稱的建構函式

Pizza piz = new Pizza("good cheese", "spicy");
Pizza piz2 = new Pizza("good cheese");




// 先建立類別
class Pizza
{
    public string cheese;
    public string taste;

    // 在類別裡做建構函式
    public Pizza(string cheese, string taste)// 初始化的時候帶入參數
    { 
       this.cheese = cheese;
       this.taste = taste;
    
    }

    public Pizza(string cheese)
    { 
      this.cheese=cheese;
    
    }

}