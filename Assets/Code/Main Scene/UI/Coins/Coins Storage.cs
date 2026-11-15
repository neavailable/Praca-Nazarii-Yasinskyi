namespace MainScene.UI.Coins
{
    public class CoinsStorage
    {
        public int Coins { get; private set; }


        public CoinsStorage() => Coins = 0;
        
        public void AddCoins(int addendCoins) => Coins += addendCoins;
    }
}