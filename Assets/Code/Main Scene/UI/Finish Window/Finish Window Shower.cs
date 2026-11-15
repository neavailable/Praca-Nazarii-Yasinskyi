using UnityEngine;
using Zenject;
using MainScene.Ball.BallDestroyer;


namespace MainScene.UI.FinishWindow
{
    public class FinishWindowShower : MonoBehaviour
    {
        private FinishWindowShowerLogic _finishWindowShowerLogic;
        
        
        [Inject]
        private void Constructor
        (
            BallDestroyer ballDestroyer,
            ClassesParameters.LooseWindow looseWindow
        )
        {
            _finishWindowShowerLogic = new 
           (
               ballDestroyer,
               looseWindow.Object
           );
        }
        
        private void OnDestroy() => _finishWindowShowerLogic.Dispose();
    }
}