using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject;
using R3;
using MainScene.ClassesParameters;
using MainScene.Coin;


namespace Infrastructure.MainScene.Factories.Coins
{
    public class CoinsFactory : IDisposable
    {
        public readonly Subject<Unit> LastCoinCollected;
        
        private readonly Stack<GameObject> _coins;
        private readonly GameObject _coinPrefab;
        private readonly DiContainer _diContainer;
        private readonly Stack<IDisposable> _coinsDestroyed;
        private readonly int _maxCoinsNumber;
        

        public CoinsFactory
        (
            DiContainer diContainer, 
            AbstractGameObjectParametr objectPrefab, 
            Stack<CoinTransform> coins
        ) 
        {
            _coinPrefab  = objectPrefab.Object;
            _diContainer = diContainer;
            
            LastCoinCollected = new ();
            _coins            = new ();
            _coinsDestroyed   = new ();
            
            FillCoins(coins);
            
            _maxCoinsNumber = coins.Count;
            
            BindCoinsTriggers();
            EnableAllCoins(coins);
        }

        public void Dispose()
        {
            while (!_coinsDestroyed.IsEmpty()) _coinsDestroyed.Pop().Dispose();
        }
        
        
        private void Pop(Vector3 position, Vector3 normal)
        {
            GameObject coin = _coins.Pop();
            
            coin.transform.position = position;
            coin.transform.rotation = normal == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(normal);
        }

        private void FillCoins(Stack<CoinTransform> coins)
        {
            foreach (CoinTransform _ in coins)
            {
                GameObject coin = InstantiateCoin();
                _coins.Push(coin);
                SubscribeOnDestroying(coin.GetComponent<CoinDestroyer>());
            }
        }

        private void SubscribeOnDestroying(CoinDestroyer coinDestroyer)
        {
            _coinsDestroyed.Push(coinDestroyer.CoinDestroyed.Subscribe(PushCoin));
        }

        private void PushCoin(GameObject coin)
        {
            _coins.Push(coin);
            if (_coins.Count == _maxCoinsNumber) LastCoinCollected.OnNext(Unit.Default);
        }
        
        private GameObject InstantiateCoin()
        {
            return _diContainer.InstantiatePrefab
            (
                _coinPrefab,
                Vector3.zero,
                Quaternion.identity,
                null
            );
        }

        private void BindCoinsTriggers()
        {
            Stack<Subject<int>> coinsTriggersSubjects = new ();
            
            foreach (GameObject coin in _coins)
            {
                coinsTriggersSubjects.Push
                (
                    coin.gameObject.GetComponent<CoinTrigger>().UpdateStorage
                );
            }

            _diContainer
                .Bind<CoinTriggersSubjects>()
                .FromInstance(new CoinTriggersSubjects(coinsTriggersSubjects))
                .AsSingle();
        }
        
        private void EnableAllCoins(Stack<CoinTransform> coins)
        {
            foreach (CoinTransform coin in coins)
            {
                Pop(coin.Object.transform.position, Vector3.zero);
            }
        }
    }
}
