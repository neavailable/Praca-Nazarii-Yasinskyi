using Zenject;
using MainScene.LineRender;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindLineRenderDrawer : Binder
    {
        private readonly LineRenderDrawer _lineRenderDrawer;
        
        
        public BindLineRenderDrawer
        (
            DiContainer container,
            LineRenderDrawer lineRenderDrawer
        ) : base(container)
        {
            _lineRenderDrawer = lineRenderDrawer;
        }

        public override void Bind()
        {
            Container.Bind<LineRenderDrawer>().FromInstance(_lineRenderDrawer);
        }
    }
}