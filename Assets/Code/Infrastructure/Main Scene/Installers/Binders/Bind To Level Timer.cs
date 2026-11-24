using Zenject;
using AllScenes.Statistics;
using Configuration.Configurations;
using Configuration.Data;
using MainScene.UI.Timer;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToLevelTimer : Binder
    {
        private readonly LevelStatistics _levelStatistics;
        private readonly LevelRulesData _levelRules;


        public BindToLevelTimer(DiContainer container, string levelName)
            : base(container)
        {
            _levelStatistics = new LevelStatistics(levelName);
            _levelRules      = new LevelRulesConfiguration(levelName).Load();
        }

        public override void Bind()
        {
            BindLevelRules();
            BindLevelStatistics();
            BindLevelTimerLogic();
        }


        private void BindLevelRules()
        {
            Container.Bind<LevelRulesData>().FromInstance(_levelRules).AsSingle();
        }

        private void BindLevelStatistics()
        {
            Container.Bind<LevelStatistics>().FromInstance(_levelStatistics).AsSingle();
        }

        private void BindLevelTimerLogic()
        {
            Container
                .Bind<LevelTimerLogic>()
                .FromInstance(new LevelTimerLogic(_levelStatistics, _levelRules))
                .AsSingle();
        }
    }
}
