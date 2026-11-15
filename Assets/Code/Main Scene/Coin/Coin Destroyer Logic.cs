using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;


namespace MainScene.Coin
{
    public class CoinDestroyerLogic : IDisposable
    {
        public readonly Subject<GameObject> CoinDestroyed;

        private readonly Transform _transform;
        private readonly CoinTrigger _coinTrigger;
        private readonly ShrinkAnimation _shrinkAnimation;


        public CoinDestroyerLogic
        (
            Transform transform,
            CoinTrigger coinTrigger,
            float shrinkingSpeed,
            float minScale
        )
        {
            _transform       = transform;
            _coinTrigger     = coinTrigger;
            _shrinkAnimation = new ShrinkAnimation(transform, shrinkingSpeed, minScale);

            CoinDestroyed = new ();

            _coinTrigger.BallCollectCoin += StartDestroying;
        }

        public void Dispose() => _coinTrigger.BallCollectCoin -= StartDestroying;


        private async UniTaskVoid Shrinking()
        {
            await _shrinkAnimation.Play();
            CoinDestroyed.OnNext(_transform.gameObject);

            _transform.gameObject.SetActive(false);
        }

        private void StartDestroying() => Shrinking().Forget();
    }
}
