using Configuration.Data;
using MainScene.ClassesParameters;
using UnityEngine;
using TMPro;
using Zenject;
using MainScene.UI.Coins.MVVM;


namespace MainScene.UI.Coins
{
    [RequireComponent(typeof(TMP_Text))]
    public class CoinsText : MonoBehaviour
    {
        private CoinsModel _coinsModel;
        private CoinsViewModel _coinsViewModel;
        private CoinsView _coinsView;
        
        
        [Inject]
        private void Constructor
        (
            CoinTriggersSubjects coinTriggersSubjects, 
            UICoinData UICoinData
        )
        {
            _coinsModel     = new CoinsModel(coinTriggersSubjects.CoinsTriggersSubjects);
            _coinsViewModel = new CoinsViewModel(_coinsModel, UICoinData.ChangeSpeed);
            _coinsView      = new CoinsView
            (
                GetComponent<TMP_Text>(), 
                _coinsViewModel,
                UICoinData.ScaleFontSpeed,
                UICoinData.MaxFontSize
            );
        }
        
        private void OnDestroy()
        {
            _coinsView     .Dispose();            
            _coinsViewModel.Dispose();
            _coinsModel    .Dispose(); 
        }
    }
}