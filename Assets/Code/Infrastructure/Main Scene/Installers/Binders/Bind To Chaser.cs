using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToChaser : Binder
    {
        private readonly ChaserData _chaserData;


        public BindToChaser(DiContainer container, ChaserData chaserData)
            : base(container)
        {
            _chaserData = chaserData;
        }

        public override void Bind() => BindChaserData();


        private void BindChaserData()
        {
            Container.Bind<ChaserData>().FromInstance(_chaserData).AsSingle();
        }
    }
}
