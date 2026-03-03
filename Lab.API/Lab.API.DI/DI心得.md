# DI 的學習心得

### 我理解的 DI 注入的目的是為了讓程式不要硬編碼 , 讓各種方法可以自由抽換 , 這樣要改某一個功能時就不用一直修改舊的

1. 我先寫一個 Class 繼承 InterFace , 再來在 Progarm 註冊

```csharp

public interface ISample
{
    int Id { get; }
}

public class Sample : ISample
{
    private static int _counter;

    private int _id;

    public Sample()
    {
        _id = ++_counter;
    }

    public int Id => _id;
}

 
// 註冊 Interface , 實作 class
builder.Services.AddScoped<ISample, Sample>();
```

2. 注入 DI 

```csharp
public readonly ISample _sample;

public HomeController(ISample sample)
{
    _sample = sample;
}

public string Index()
{
    // 就可以看到 Sample 方法拿到的值
    return $"[ISample]\r\n"
        + $"Id: {_sample.Id}\r\n"
        + $"HashCode: {_sample.GetHashCode()}\r\n"
        + $"Tpye: {_sample.GetType()}";
}
```

3. 再來試試看三種不同的 Service , 牠們的生命周期都不一樣

```csharp

builder.Services.AddScoped<ISampleScoped, Sample>(); 
// Scope : 每個請求就 new 一個實例,也是比較常使用的
builder.Services.AddTransient<ISampleTransient, Sample>(); 
// Transient : 每次注入就 new 一個實例
builder.Services.AddSingleton<ISampleSingleton, Sample>(); 
// Singleton : 在整個程式運行期間只會有一個實例
```

4. 我翻到另一篇文章講的也很不錯 , 我再試一個例子

```csharp

// 1. 定義插頭的介面 , 讓他有個方法 , 意指插插座連結到電源
public interface IElectricalPlug
{
    void Connect();
}


// 2. 再來定義使用插頭的插座
public class Socket
{
    private readonly IElectricalPlug _plug;

    public Socket(IElectricalPlug plug)
    {
        if (plug == null)
        {
            throw new ArgumentNullException("plug是空的!");
        }
        _plug = plug;
    }

    public void SendPower()
    {
        this._plug.Connect();
    }
}


// 3. 再試試實作 HairDryerPlug 類別

public class HairDryerPlug : IElectricalPlug
{
    public void Connect()
    {
        Console.WriteLine("HairDryerPlug connected!\n");
    }
}


 // 4. 在呼叫就有了! 這樣之後要實作別的類別,只要有 IElectricalPlug 介面都行
 System.Console.WriteLine("使用吹風機插頭");
 IElectricalPlug hairDryerPlug = new HairDryerPlug();
 var socket = new Socket(hairDryerPlug);
 socket.SendPower();


 // 5. 再做一個身分驗證 , 確認是母親身分才允許使用
public class SecruHairDyperPlug : IElectricalPlug
{
    // 注入插座
    public IElectricalPlug plug { get; set; }

    // 識別證
    public string identity { get; set; }

    public SecruHairDyperPlug(IElectricalPlug plug, string identity)
    {
        this.plug = plug;
        this.identity = identity;
    }

    public void Connect()
    {
        // 假設身分是母親才能用
        if (identity == "Mom")
        {
            Console.WriteLine("身分驗證成功");
            plug.Connect();
        }
    }
}


// 6. 這樣再把使用吹風機插頭的介面跟母親身分注入 , 就可以了
Console.WriteLine("使用經身分驗證的安全吹風機插頭");
IElectricalPlug secureHairDryerPlug = new SecruHairDyperPlug(hairDryerPlug, "Mom");
var socketDecorated = new Socket(secureHairDryerPlug);
socketDecorated.SendPower();
```


5. 介面隔離實作練習

```csharp

 public interface IEmailNotifier
 {
     Task SendEmailAsync(string to, string title, string body);
 }

 public interface ISmsNotifier
 {
     Task SendSmsAsync(string phonenumber, string username);
 }

 public interface IMessageNotifier
 {
     Task SendMessageAsync(int userId, string username, string body);
 }

 private readonly IEmailNotifier _emailNotifier;

 private readonly IMessageNotifier _messageNotifier;

 public EmailController(IEmailNotifier notifier, IMessageNotifier messageNotifier)
 {
     _emailNotifier = notifier;
     _messageNotifier = messageNotifier;
 }

  
 public async Task ConfrimOrderAsync(Order order)
 {
     await _emailNotifier.SendEmailAsync(
         order.CustomerEmail,
         "訂單確認",
         $"訂購的商品{order.Name}已到達"
     );
 }
 
```