using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToHeal : Binder
    {
        private readonly HealData _healData;


        public BindToHeal(DiContainer container, HealData healData)
            : base(container)
        {
            _healData = healData;
        }

        public override void Bind() => BindHealData();


        private void BindHealData()
        {
            Container.Bind<HealData>().FromInstance(_healData).AsSingle();
        }
    }
}
