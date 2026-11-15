using Zenject;
using Configuration.Data;
using MainScene.Wind;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToWind : Binder
    {
        private readonly WindData _windData;


        public BindToWind(DiContainer container, WindData windData)
            : base(container)
        {
            _windData = windData;
        }

        public override void Bind()
        {
            BindWindData();
            BindLevelWind();
        }


        private void BindWindData()
        {
            Container.Bind<WindData>().FromInstance(_windData).AsSingle();
        }

        private void BindLevelWind()
        {
            Container.Bind<LevelWind>().FromInstance(new LevelWind(_windData)).AsSingle();
        }
    }
}
