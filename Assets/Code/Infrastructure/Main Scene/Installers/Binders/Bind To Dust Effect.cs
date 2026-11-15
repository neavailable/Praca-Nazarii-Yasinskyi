using Zenject;
using Configuration.Data;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToDustEffect : Binder
    {
        private readonly DustEffectData _dustEffectData;
        
        
        public BindToDustEffect(DiContainer container, DustEffectData dustEffectData) : base(container)
        {
            _dustEffectData = dustEffectData;
        }

        public override void Bind() => BindDustEffectData();


        private void BindDustEffectData()
        {
            Container.Bind<DustEffectData>().FromInstance(_dustEffectData);
        }
    }
}