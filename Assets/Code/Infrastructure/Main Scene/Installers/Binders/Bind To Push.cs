using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToPush : Binder
    {
        private readonly PushData _pushData;


        public BindToPush(DiContainer container, PushData pushData)
            : base(container)
        {
            _pushData = pushData;
        }

        public override void Bind() => BindPushData();


        private void BindPushData()
        {
            Container.Bind<PushData>().FromInstance(_pushData).AsSingle();
        }
    }
}
