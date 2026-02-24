


List<Player> players = new List<Player>();

Player player1 = new Player("Andy");
Player player2 = new Player("Eddy");

players.Add(player1);
players.Add(player2);

foreach (Player p in players)
{
    Console.WriteLine(p);
}

class Player
{
    public string username;

    public Player(string username)
    { 
       this.username = username;
    }
    // 把字串方法覆蓋
    public override string ToString()
    {
        return username;
    }
}