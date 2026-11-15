using System;
using R3;


namespace MainScene.UI.InformationPanel.AbstractMVVM
{
    public abstract class AbstractModel : IDisposable
    {
        public readonly ReactiveProperty<float> Data;

        private readonly IDisposable _changeData;


        protected AbstractModel(ReactiveProperty<float> meshRotation)
        {
            Data = new ();

            _changeData = meshRotation.Subscribe(ChangeData);
        }


        public void Dispose() => _changeData.Dispose();
        
        
        private void ChangeData(float data) => Data.Value = data;
    }
}