using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Coin.CoinValue
{
    public class CoinValue : MonoBehaviour
    {
        public int Coins => _coins; 
        
        private int _coins;


        [Inject]
        private void Constructor(CoinValueData coinValueData)
        {
            _coins = coinValueData.CoinValue;
        }
    }
}