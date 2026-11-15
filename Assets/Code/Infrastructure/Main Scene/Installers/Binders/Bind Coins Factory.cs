using Zenject;
using Infrastructure.MainScene.Factories.Coins;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindCoinsFactory : Binder
    {
        private readonly CoinsFactoryCreator _coinsFactoryCreator;


        public BindCoinsFactory(DiContainer container, CoinsFactoryCreator coinsFactoryCreator)
            : base(container)
        {
            _coinsFactoryCreator = coinsFactoryCreator;
        }

        public override void Bind()
        {
            Container.Bind<CoinsFactoryCreator>().FromInstance(_coinsFactoryCreator).AsSingle();
        }
    }
}