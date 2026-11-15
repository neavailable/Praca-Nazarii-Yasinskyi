using System;
using TMPro;
using R3;


namespace MainScene.UI.InformationPanel.AbstractMVVM
{
    public abstract class AbstractView : IDisposable
    {
        private readonly TMP_Text _text;
        private readonly IDisposable _updateView;
        
        
        protected AbstractView(TMP_Text text, ReactiveProperty<string> textChanger)
        {
            _text = text;
            
            _updateView = textChanger.Subscribe(UpdateText);
        }


        private void UpdateText(string text) => _text.text = text;
        
        
        public void Dispose() => _updateView.Dispose();
    }
}