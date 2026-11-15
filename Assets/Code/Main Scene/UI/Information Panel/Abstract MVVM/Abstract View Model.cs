using System;
using R3;


namespace MainScene.UI.InformationPanel.AbstractMVVM
{
    public abstract class AbstractViewModel : IDisposable
    {
        public readonly ReactiveProperty<string> Text;
    
        private readonly string _standardTextPart;
        private readonly IDisposable _changeText;


        protected AbstractViewModel(ReactiveProperty<float> data, string standardTextPart)
        {
            _standardTextPart = standardTextPart;

            Text = new("");

            _changeText = data.Subscribe(ChangeText);
        }


        public void Dispose() => _changeText.Dispose();

        
        private void ChangeText(float data)
        {
            if (Text.Value == data.ToString()) return;
            
            Text.Value = _standardTextPart + " " + (float) Math.Round(value: data, digits: 1);
        }
    }
}