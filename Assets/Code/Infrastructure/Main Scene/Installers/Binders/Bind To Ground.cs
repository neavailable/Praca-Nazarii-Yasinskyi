using Configuration.Data;
using Zenject;



namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToGround : Binder
    {
        private readonly GroundData _groundData;
        
        
        public BindToGround(DiContainer container, GroundData groundData) 
            : base(container)
        {
            _groundData = groundData;
        }

        public override void Bind() => BindGroundData();


        private void BindGroundData()
        {
            Container.Bind<GroundData>().FromInstance(_groundData);
        }
    }
}