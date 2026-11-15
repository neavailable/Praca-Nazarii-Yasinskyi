using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToMinimap : Binder
    {
        private readonly MinimapData _minimapData;


        public BindToMinimap(DiContainer container, MinimapData minimapData)
            : base(container)
        {
            _minimapData = minimapData;
        }

        public override void Bind() => BindMinimapData();


        private void BindMinimapData()
        {
            Container.Bind<MinimapData>().FromInstance(_minimapData).AsSingle();
        }
    }
}
