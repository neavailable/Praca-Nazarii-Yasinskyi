using Zenject;


namespace Infrastructure.AllScenes.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        private readonly LoadLevel.LoadLevel _loadLevel = new ();
        
        
        public override void InstallBindings()
        {
            Container.Bind<LoadLevel.LoadLevel>().FromInstance(_loadLevel).AsSingle();
        }
    }
}