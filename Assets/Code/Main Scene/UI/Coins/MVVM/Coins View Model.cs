using System;
using R3;
using MainScene.UI.Animations;


namespace MainScene.UI.Coins.MVVM
{
    public class CoinsViewModel : IDisposable
    {
        public readonly ReactiveProperty<string> CoinsText;
        public readonly SmoothChangingIntTextAnimation SmoothChangingIntTextAnimation;

        private readonly CoinsModel _coinsModel;


        public CoinsViewModel(CoinsModel coinsModel, float changeSpeed)
        {
            _coinsModel = coinsModel;
            
            CoinsText                      = new("0");
            SmoothChangingIntTextAnimation = new(CoinsText, changeSpeed);

            _coinsModel.AddCoinsAction += AddCoins;
        }

        public void Dispose() => _coinsModel.AddCoinsAction -= AddCoins;


        private void AddCoins(int coins)
        {
            SmoothChangingIntTextAnimation.Change(coins).Forget();
        }
    }
}