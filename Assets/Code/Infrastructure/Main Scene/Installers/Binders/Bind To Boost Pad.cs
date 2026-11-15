using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToBoostPad : Binder
    {
        private readonly BoostPadData _boostPadData;


        public BindToBoostPad(DiContainer container, BoostPadData boostPadData)
            : base(container)
        {
            _boostPadData = boostPadData;
        }

        public override void Bind() => BindBoostPadData();


        private void BindBoostPadData()
        {
            Container.Bind<BoostPadData>().FromInstance(_boostPadData).AsSingle();
        }
    }
}
