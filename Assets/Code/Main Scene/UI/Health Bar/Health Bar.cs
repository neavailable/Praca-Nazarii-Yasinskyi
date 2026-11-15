using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Configuration.Data;
using MainScene.UI.HealthBar.MVVM;


namespace MainScene.UI.HealthBar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Slider _slider;

        private HealthBarTextViewModel _healthBarTextViewModel;
        private HealthBarTextView _healthBarTextView;
        private HealthBarViewModel _healthBarViewModel;
        private HealthBarView _healthBarView;
        
        
        [Inject]
        private void Constructor(HealthBarModel healthBarModel, UIHealthData UIHealthData)
        {
            _healthBarTextViewModel = new (healthBarModel, UIHealthData.Delay);
            _healthBarTextView      = new 
            (
                _healthText, 
                _healthBarTextViewModel,
                UIHealthData.MaxFontSize
            );
            _healthBarViewModel     = new (healthBarModel);
            _healthBarView          = new 
            (
                _slider,
                _healthBarViewModel.Health,
                _healthBarViewModel.DefalultHealth
            );
        }

        private void OnDestroy()
        {
            _healthBarView         .Dispose();
            _healthBarViewModel    .Dispose();
            _healthBarTextView     .Dispose();
            _healthBarTextViewModel.Dispose();
        }
    }
}