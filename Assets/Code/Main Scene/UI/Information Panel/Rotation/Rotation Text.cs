using UnityEngine;
using TMPro;
using Zenject;
using MainScene.Ball.UIDisplayable;
using MainScene.UI.InformationPanel.Rotation.MVVM;


namespace MainScene.UI.InformationPanel.Rotation
{
    [RequireComponent(typeof(TMP_Text))]
    public class RotationText : MonoBehaviour
    {
        private RotationModel _rotationModel;
        private RotationViewModel _rotationViewModel;
        private RotationView _rotationView;          
        
        
        [Inject]
        private void Constructor(BallMeshRotationSender ballMeshRotationSender)
        {
            _rotationModel          = new (ballMeshRotationSender.MeshRotation);
            _rotationViewModel      = new (_rotationModel.Data);
            _rotationView           = new (GetComponent<TMP_Text>(), _rotationViewModel.Text);
        }

        private void OnDestroy()
        {
            _rotationView     .Dispose();
            _rotationViewModel.Dispose();
            _rotationModel    .Dispose();
        }
    }
}