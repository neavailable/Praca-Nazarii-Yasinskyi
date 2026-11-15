using UnityEngine;


namespace MainScene.UI.Animations
{
    public class ScaleTextAnimation
    {
        private readonly float _defaultFontSize, _scaleFontSpeed, _maxFontSize;
        private float _currentFontSize;
        private int _textScale;
        
        
        public ScaleTextAnimation
        (
            float defaultFontSize, 
            float scaleFontSpeed, 
            float maxFontSize
        )
        {
            _defaultFontSize = defaultFontSize;
            _scaleFontSpeed  = scaleFontSpeed;
            _maxFontSize     = maxFontSize;
            _currentFontSize = _defaultFontSize;
            _textScale       = 1;
        }

        public float NewTextSize()
        {
            if (ShouldChangeScale()) _textScale = -_textScale;
            
            _currentFontSize += _scaleFontSpeed * Time.fixedDeltaTime * _textScale;
            
            return _currentFontSize;
        }

        public float GetDefaultFontSize()
        {
            return _currentFontSize = _defaultFontSize;
        }


        private bool ShouldChangeScale()
        {
            return _currentFontSize > _maxFontSize || _currentFontSize < _defaultFontSize;
        }
    }
}