using System;
using TMPro;
using R3;


namespace MainScene.UI.HealthBar.MVVM
{
    public class HealthBarTextView : IDisposable
    {
        private readonly TMP_Text _text;
        private readonly IDisposable _updateText;
        private readonly IDisposable _increaseFontSize, _decreaseFontSize;
        private readonly float _defaultFontSize, _maxFontSize;
        

        public HealthBarTextView
        (
            TMP_Text text, 
            HealthBarTextViewModel healthTextViewModel,
            float maxFontSize
        )
        {
            _text            = text; 
            _defaultFontSize = _text.fontSize;
            _maxFontSize     = maxFontSize;
            _text.text       = healthTextViewModel.DefaultText;

            _updateText       = 
                healthTextViewModel.HealthText.Subscribe(UpdateText);
            _increaseFontSize = 
                healthTextViewModel.IncreaseFontSize.Subscribe(IncreaseFontSize);
            _decreaseFontSize = 
                healthTextViewModel.DecreaseFontSize.Subscribe(DecreaseFontSize);
        }

        public void Dispose()
        {
            _decreaseFontSize.Dispose();
            _increaseFontSize.Dispose();
            _updateText      .Dispose();
        }


        private void UpdateText(string text) => _text.text = text;

        private void IncreaseFontSize(Unit _) => _text.fontSize = _maxFontSize;
        
        private void DecreaseFontSize(Unit _) => _text.fontSize = _defaultFontSize;
    }
}