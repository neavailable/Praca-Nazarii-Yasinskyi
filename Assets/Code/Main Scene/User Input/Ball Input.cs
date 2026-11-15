using UnityEngine;


namespace MainScene.UserInput
{
    public class BallInput : MonoBehaviour
    {
        public BallInputLogic BallInputLogic { get; private set; }
        
        
        private void OnEnable() => BallInputLogic = new BallInputLogic();
        
        private void OnDisable() => BallInputLogic.Dispose();
    }
}