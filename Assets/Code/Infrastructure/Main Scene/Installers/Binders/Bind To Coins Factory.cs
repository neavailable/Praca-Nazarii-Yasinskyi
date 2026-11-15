using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;
using MainScene.Coin;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToCoinsFactory : Binder
    {
        private readonly AllCoinsMarker _allCoinsMarker;
        private readonly GameObject _coinPrefab;


        public BindToCoinsFactory
        (
            DiContainer container,
            AllCoinsMarker allCoinsMarker,
            GameObject coinPrefab
        ) : base(container)
        {
            _allCoinsMarker = allCoinsMarker;    
            _coinPrefab     = coinPrefab;
        }
        
        public override void Bind()
        {
            BindCoinPrefab();
            BindCoinTransforms(CreateCoinTransforms());
        }
        

        private void BindCoinPrefab()
        {
            Container.Bind<CoinPrefab>().
                FromInstance(new CoinPrefab(_coinPrefab)).AsSingle();
        }

        private Stack<CoinTransform> CreateCoinTransforms()
        {
            Stack<CoinTransform> coinsTransforms = new ();
            foreach (Transform child in _allCoinsMarker.transform)
            {
                coinsTransforms.Push(new CoinTransform(child.gameObject));
            }
            
            return coinsTransforms;
        }

        private void BindCoinTransforms(Stack<CoinTransform> coinsTransforms)
        {
            Container.Bind<Stack<CoinTransform>>().
                FromInstance(coinsTransforms).AsSingle();
        }
    }
}