using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToCactus : Binder
    {
        private readonly CactusMovementData _cactusMovementData;
        private readonly CactusDamageData _cactusDamageData;
        
        
        public BindToCactus
        (
            DiContainer container,
            CactusMovementData cactusMovementData,
            CactusDamageData cactusDamageData
        ) : base(container)
        {
            _cactusMovementData = cactusMovementData;
            _cactusDamageData   = cactusDamageData;
        }

        public override void Bind()
        {
            BindCactusMovementData();
            BindCactusDamageData();
        }


        private void BindCactusMovementData()
        {
            Container.Bind<CactusMovementData>()
                .FromInstance(_cactusMovementData).AsSingle();
        }

        private void BindCactusDamageData()
        {
            Container.Bind<CactusDamageData>()
                .FromInstance(_cactusDamageData).AsSingle();
        }
    }
}