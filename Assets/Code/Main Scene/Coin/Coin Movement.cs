using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Coin
{
    public class CoinMovement : MonoBehaviour
    {
        private CoinMovementLogic _coinMovementLogic;
        private CoinMovementData _coinMovementData;
        
        
        [Inject]
        private void Constructor(CoinMovementData coinMovementData)
        {
            _coinMovementData = coinMovementData;
        }

        private void Start()
        {
            _coinMovementLogic = new CoinMovementLogic
            (
                transform,
                _coinMovementData.YSpeed,
                _coinMovementData.YBorder
            );
        }
        
        private void FixedUpdate() => _coinMovementLogic.Move();
    }
}