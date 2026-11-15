using Zenject;
using MainScene.Ground;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindFromGround : Binder
    {
        private readonly GroundMarker _groundMarker;
        
        
        public BindFromGround(DiContainer container, GroundMarker groundMarker) 
            : base(container)
        {
            _groundMarker = groundMarker;
        }
        
        public override void Bind()
        {
            Container.Bind<GroundMarker>().FromInstance(_groundMarker);
        }
    }
}