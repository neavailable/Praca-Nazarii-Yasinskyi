using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToWall : Binder
    {
        private readonly WallData _wallData; 
        
        
        public BindToWall(DiContainer container, WallData wallData) : base(container)
        {
            _wallData = wallData;
        }

        public override void Bind() => BindWallData();


        private void BindWallData()
        {
            Container.Bind<WallData>().FromInstance(_wallData);
        }
    }
}