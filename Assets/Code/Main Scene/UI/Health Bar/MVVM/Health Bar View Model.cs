using System;
using R3;


namespace MainScene.UI.HealthBar.MVVM
{
    public class HealthBarViewModel : IDisposable
    {
        public readonly ReactiveProperty<float> Health;
        public readonly float DefalultHealth;

        private const int percent = 100;
        private readonly HealthBarModel _healthBarModel;


        public HealthBarViewModel(HealthBarModel healthBarModel)
        {
            _healthBarModel = healthBarModel;

            Health = new ((float) healthBarModel.DefaultHealth / percent);
            DefalultHealth = Health.Value;
                
            _healthBarModel.HealthChanged += UpdateSlider;
        }

        public void Dispose() => _healthBarModel.HealthChanged -= UpdateSlider;


        private void UpdateSlider(int health)
        {
            Health.Value = (float) health / percent;
        }
    }
}