using UnityEngine;
using TMPro;
using Zenject;
using MainScene.Ball.UIDisplayable;
using MainScene.UI.InformationPanel.Speed.MVVM;


namespace MainScene.UI.InformationPanel.Speed
{
    [RequireComponent(typeof(TMP_Text))]
    public class SpeedText : MonoBehaviour
    {
        private SpeedModel _speedModel;
        private SpeedViewModel _speedViewModel;
        private SpeedView _speedView;


        [Inject]
        private void Constructor(BallSpeedSender ballSpeedSender)
        {
            _speedModel     = new (ballSpeedSender.Speed);
            _speedViewModel = new (_speedModel.Data);
            _speedView      = new (GetComponent<TMP_Text>(), _speedViewModel.Text);
        }

        private void OnDestroy()
        {
            _speedView     .Dispose();
            _speedViewModel.Dispose();
            _speedModel    .Dispose();
        }
    }
}