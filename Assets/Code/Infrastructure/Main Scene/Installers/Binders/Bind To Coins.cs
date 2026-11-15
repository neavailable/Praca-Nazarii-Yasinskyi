using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToCoins : Binder
    {
        private readonly CoinValueData _coinValueData;
        private readonly CoinMovementData _coinMovementData;
        private readonly CoinRotationData _coinRotationData;
        private readonly CoinDestroyerData _coinDestroyerData;
        

        public BindToCoins
        (
            DiContainer container,
            CoinValueData coinValueData,
            CoinMovementData coinMovementData,
            CoinRotationData coinRotationData, 
            CoinDestroyerData coinDestroyerData
        ) : base(container)
        {
            _coinValueData     = coinValueData;
            _coinMovementData  = coinMovementData;
            _coinRotationData  = coinRotationData;
            _coinDestroyerData = coinDestroyerData;
        }

        public override void Bind()
        {
            BindCoinValueData();
            BindCoinMovementData();
            BindCoinRotationData();
            BindCoinDestroyerData();
        }

        private void BindCoinValueData()
        {
            Container.Bind<CoinValueData>().FromInstance(_coinValueData);
        }

        private void BindCoinMovementData()
        {
            Container.Bind<CoinMovementData>().FromInstance(_coinMovementData);
        }

        private void BindCoinRotationData()
        {
            Container.Bind<CoinRotationData>().FromInstance(_coinRotationData);
        }

        private void BindCoinDestroyerData()
        {
            Container.Bind<CoinDestroyerData>().FromInstance(_coinDestroyerData);
        }
    }
}