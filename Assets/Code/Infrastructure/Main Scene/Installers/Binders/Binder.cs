using Zenject;


namespace Infrastructure.MainScene.Installers.Binders
{
    public abstract class Binder
    {
        protected readonly DiContainer Container;

        
        protected Binder(DiContainer container) => Container = container;

        
        public abstract void Bind();
    }
}