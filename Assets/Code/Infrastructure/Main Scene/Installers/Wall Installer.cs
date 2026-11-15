using System.Collections.Generic;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;


namespace Infrastructure.MainScene.Installers
{
    public class WallInstaller : MonoInstaller
    {
        private readonly List<Binder> _binders = new ();
        
        
        public override void InstallBindings()
        {
            CreateBinders();
            Bind();
        }
        

        private void CreateBinders()
        {
            _binders.Add
            (
                new BindToWall(Container, new WallConfiguration().Load())
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}