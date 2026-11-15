using Zenject;
using Configuration.Data;
using Configuration.Configurations;


namespace Infrastructure.MainMenu
{
    public class WallMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindWallData();
        }


        private void BindWallData()
        {
            Container.Bind<WallData>()
                .FromInstance(new WallMenuConfiguration().Load()).AsSingle();
        }
    }
}