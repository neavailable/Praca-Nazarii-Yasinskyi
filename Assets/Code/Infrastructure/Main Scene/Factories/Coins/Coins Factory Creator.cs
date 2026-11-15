using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Factories.Coins
{
    public class CoinsFactoryCreator : MonoBehaviour
    {
        public CoinsFactory CoinsFactory { get; private set; }


        [Inject]
        private void Constructor
        (
            DiContainer diContainer,
            CoinPrefab coinPrefab, 
            Stack<CoinTransform> coinTransforms
        )
        {
            CoinsFactory = new CoinsFactory(diContainer, coinPrefab, coinTransforms);
        }
    }
}