using System;
using UnityEngine;
using R3;
using MainScene.Ball.BallMarker;


namespace MainScene.Coin
{
    public class CoinTrigger : MonoBehaviour
    {
        public Action BallCollectCoin;
        public Subject<int> UpdateStorage;

        private CoinValue.CoinValue _coinValue;
        private bool _coinWasCollected;

        
        private void Awake() => UpdateStorage = new ();
            
        private void Start()
        {
            _coinWasCollected = false;
            
            _coinValue = GetComponent<CoinValue.CoinValue>();
            
            UpdateStorage.OnNext(0);
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out BallMarker _) && !_coinWasCollected)
            {
                BallCollectCoin?.Invoke();
                UpdateStorage.OnNext(_coinValue.Coins);
                _coinWasCollected = true;
            }
        }
    }
}