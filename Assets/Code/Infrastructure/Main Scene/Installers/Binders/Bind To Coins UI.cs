using Zenject;
using Configuration.Data;
using MainScene.UI.HealthBar.MVVM;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToCoinsUI : Binder
    {
        private readonly UICoinData _UICoinData;
        
        
        public BindToCoinsUI
        (
            DiContainer container, 
            UICoinData UICoinData
        ) : base(container)
        {
            _UICoinData     = UICoinData;
        }
        
        public override void Bind() => BindUICoinData();

        
        private void BindUICoinData()
        {
            Container.Bind<UICoinData>().FromInstance(_UICoinData).AsSingle();
        }
    }
}