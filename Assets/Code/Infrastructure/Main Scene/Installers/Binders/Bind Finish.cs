using MainScene.ClassesParameters;
using Zenject;

namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindFinish : Binder
    {
        private readonly Finish _finish;
        
        
        public BindFinish(DiContainer container, Finish finish) : base(container)
        {
            _finish = finish;
        }

        public override void Bind()
        {
            Container.Bind<Finish>()
                .FromInstance(_finish).AsSingle();
        }
    }
}