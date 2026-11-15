using System;
using UnityEngine;
using R3;
using Infrastructure.MainScene.Factories.Coins;


namespace MainScene.Finish
{
    public class FinishEnablerLogic : IDisposable
    {
        private readonly GameObject _finish;
        private readonly IDisposable _lastCoinCollected;


        public FinishEnablerLogic(GameObject finish, CoinsFactory coinsFactory)
        {
            _finish = finish;
            _lastCoinCollected = coinsFactory.LastCoinCollected
                .Subscribe(EnableFinish);
        }

        public void Dispose() => _lastCoinCollected.Dispose();


        private void EnableFinish(Unit _) => _finish.SetActive(true);
        
    }
}