using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToMushroom : Binder
    {
        private readonly MushroomRotationData _mushroomRotationData;
        private readonly MushroomDamageData _mushroomDamageData;
        
        
        public BindToMushroom
        (
            DiContainer container, 
            MushroomRotationData mushroomRotationData,
            MushroomDamageData mushroomDamageData
        ) : base(container)
        {
            _mushroomRotationData = mushroomRotationData;
            _mushroomDamageData   = mushroomDamageData;
        }

        public override void Bind()
        {
            BindMushroomRotationData();
            BindMushroomDamageData();
        }


        private void BindMushroomRotationData()
        {
            Container.Bind<MushroomRotationData>()
                .FromInstance(_mushroomRotationData).AsSingle();
        }
        
        private void BindMushroomDamageData()
        {
            Container.Bind<MushroomDamageData>()
                .FromInstance(_mushroomDamageData).AsSingle();
        }
    }
}