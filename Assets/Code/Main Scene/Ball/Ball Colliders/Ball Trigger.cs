using MainScene.Coin.CoinValue;
using UnityEngine;
using R3;


namespace MainScene.Ball.BallColliders
{
    public class BallTrigger : MonoBehaviour
    {
        private Subject<int> _ballColidedCoin;


        private void Awake() => _ballColidedCoin = new ();

        private void Start() => _ballColidedCoin.OnNext(0);
        
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out CoinValue coinsAmount))
            {
                _ballColidedCoin.OnNext(coinsAmount.Coins);
            }
        }
    }
}