using Zenject;
using AllScenes.Statistics;
using MainScene.UI.Timer;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToLevelTimer : Binder
    {
        private readonly LevelStatistics _levelStatistics;


        public BindToLevelTimer(DiContainer container, string levelName)
            : base(container)
        {
            _levelStatistics = new LevelStatistics(levelName);
        }

        public override void Bind()
        {
            BindLevelStatistics();
            BindLevelTimerLogic();
        }


        private void BindLevelStatistics()
        {
            Container.Bind<LevelStatistics>().FromInstance(_levelStatistics).AsSingle();
        }

        private void BindLevelTimerLogic()
        {
            Container
                .Bind<LevelTimerLogic>()
                .FromInstance(new LevelTimerLogic(_levelStatistics))
                .AsSingle();
        }
    }
}
