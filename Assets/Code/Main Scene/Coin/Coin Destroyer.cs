using UnityEngine;
using Zenject;
using R3;
using Configuration.Data;


namespace MainScene.Coin
{
    [RequireComponent(typeof(CoinTrigger))]
    public class CoinDestroyer : MonoBehaviour
    {
        public Subject<GameObject> CoinDestroyed => _coinDestroyerLogic.CoinDestroyed;
        
        private CoinDestroyerLogic _coinDestroyerLogic;
        private CoinDestroyerData _coinDestroyerData;


        [Inject]
        private void Constructor(CoinDestroyerData coinDestroyerData)
        {
            _coinDestroyerData = coinDestroyerData;
        }

        private void OnEnable()
        {
            _coinDestroyerLogic = new CoinDestroyerLogic
            (
                transform,
                GetComponent<CoinTrigger>(),
                _coinDestroyerData.ShrinkingSpeed,
                _coinDestroyerData.MinScale
            );
        }

        private void OnDisable() => _coinDestroyerLogic.Dispose();
    }
}