using System;
using UnityEngine.UI;
using R3;


namespace MainScene.UI.HealthBar.MVVM
{
    public class HealthBarView : IDisposable
    {
        private readonly Slider _slider;
        private readonly IDisposable _changeSliderValue;


        public HealthBarView
        (
            Slider slider, 
            ReactiveProperty<float> health,
            float defaultHealth
        )
        {
            _slider = slider;
            
            _slider.value = defaultHealth;
            
            _changeSliderValue = health.Subscribe(ChangeSliderValue);
        }

        public void Dispose() => _changeSliderValue.Dispose();


        private void ChangeSliderValue(float value) => _slider.value = value;
    }
}