using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToObstacles : Binder
    {
        private readonly ObstaclesData _obstaclesData;


        public BindToObstacles(DiContainer container, ObstaclesData obstaclesData)
            : base(container)
        {
            _obstaclesData = obstaclesData;
        }

        public override void Bind() => BindObstaclesData();


        private void BindObstaclesData()
        {
            Container.Bind<ObstaclesData>().FromInstance(_obstaclesData).AsSingle();
        }
    }
}
