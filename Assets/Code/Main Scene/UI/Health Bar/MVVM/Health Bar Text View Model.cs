using System;
using Cysharp.Threading.Tasks;
using R3;


namespace MainScene.UI.HealthBar.MVVM
{
    public class HealthBarTextViewModel : IDisposable
    {
        public readonly Subject<Unit> IncreaseFontSize, DecreaseFontSize; 
        public readonly ReactiveProperty<string> HealthText;
        public readonly string DefaultText;

        private readonly HealthBarModel _healthBarModel;
        private readonly float _delay;


        public HealthBarTextViewModel(HealthBarModel healthBarModel, float delay)
        {
            _healthBarModel = healthBarModel;
            _delay          = delay;
                
            IncreaseFontSize = new ();
            DecreaseFontSize = new ();
            HealthText       = new (healthBarModel.DefaultHealth.ToString());

            DefaultText = HealthText.Value;

            _healthBarModel.HealthChanged += UpdateText;
        }

        public void Dispose() => _healthBarModel.HealthChanged -= UpdateText;


        private void UpdateText(int health)
        {
            HealthText.Value = health.ToString();
            ScaleText().Forget();
        }


        private async UniTaskVoid ScaleText()
        {
            IncreaseFontSize?.OnNext(Unit.Default);
            await UniTask.WaitForSeconds(_delay);
            DecreaseFontSize?.OnNext(Unit.Default);
        }
    }
}