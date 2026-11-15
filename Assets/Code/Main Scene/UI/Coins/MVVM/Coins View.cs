using System;
using TMPro;
using R3;
using MainScene.UI.Animations;


namespace MainScene.UI.Coins.MVVM
{
    public class CoinsView : IDisposable
    {
        private readonly TMP_Text _text;
        private readonly ScaleTextAnimation _scaleTextAnimation;
        private readonly IDisposable _changeText, _pulsateText, _finishTextPulsation;
        
        
        public CoinsView
        (
            TMP_Text text,
            CoinsViewModel coinsViewModel, 
            float scaleFontSpeed, 
            float minFontSize
        )
        {
            _text = text;

            _scaleTextAnimation = new(_text.fontSize, scaleFontSpeed, minFontSize);
     
            _changeText          = coinsViewModel.CoinsText          
                .Subscribe(ChangeCoinsText);
            _pulsateText         = coinsViewModel.SmoothChangingIntTextAnimation.PulsateText          
                .Subscribe(PulsateText);
            _finishTextPulsation = coinsViewModel.SmoothChangingIntTextAnimation.FinishedTextPulsation
                .Subscribe(FinishTextPulsation);
        }
    
        public void Dispose()
        {
            _finishTextPulsation.Dispose();
            _pulsateText        .Dispose();
            _changeText         .Dispose();
        }


        private void PulsateText(Unit _) => _text.fontSize = _scaleTextAnimation.NewTextSize();

        private void FinishTextPulsation(Unit _) => _text.fontSize = _scaleTextAnimation.GetDefaultFontSize();

        private void ChangeCoinsText(string coinsText) => _text.text = coinsText;
    }
}