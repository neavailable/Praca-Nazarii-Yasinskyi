using System;
using System.Collections.Generic;
using R3;


namespace MainScene.UI.Coins.MVVM
{
    public class CoinsModel : IDisposable
    {
        public Action<int> AddCoinsAction;
        
        private readonly Stack<IDisposable> _triggersSubscriptions;
        private readonly CoinsStorage _coinsStorage;


        public CoinsModel(IEnumerable<Subject<int>> coinsTriggers)
        {
            _triggersSubscriptions = new ();
            _coinsStorage          = new ();
            
            SubscribeOnTriggers(coinsTriggers);
        }

        public void Dispose()
        {
            while (_triggersSubscriptions.Count > 0) _triggersSubscriptions.Pop().Dispose();
        }


        private void SubscribeOnTriggers(IEnumerable<Subject<int>> coinsTriggers)
        {
            foreach (Subject<int> coinTrigger in coinsTriggers)
            {
                _triggersSubscriptions.Push(coinTrigger.Subscribe(AddCoins));
            }
        }

        private void AddCoins(int coins)
        {
            _coinsStorage.AddCoins(coins);
            AddCoinsAction?.Invoke(_coinsStorage.Coins);
        } 
    }
}