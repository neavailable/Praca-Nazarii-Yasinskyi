using Zenject;
using MainScene.LineRender;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToLineRenderDrawer : Binder
    {
        private readonly LineRenderEnabler _lineRenderEnabler;
        
        
        public BindToLineRenderDrawer
        (
            DiContainer container,
            LineRenderEnabler lineRenderEnabler
        ) : base(container)
        {
            _lineRenderEnabler = lineRenderEnabler;
        }

        public override void Bind()
        {
            Container.Bind<LineRenderEnabler>().FromInstance(_lineRenderEnabler);
        }
    }
}