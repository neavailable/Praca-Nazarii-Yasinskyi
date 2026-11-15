using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToEnemy : Binder
    {
        private readonly EnemyBounceData _enemyBounceData;


        public BindToEnemy(DiContainer container, EnemyBounceData enemyBounceData) 
            : base(container)
        {
            _enemyBounceData = enemyBounceData;
        }
        
        public override void Bind() => BindEnemyBounceData();


        private void BindEnemyBounceData()
        {
            Container.Bind<EnemyBounceData>()
                .FromInstance(_enemyBounceData).AsSingle();
        }
    }
}