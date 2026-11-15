using UnityEngine;


namespace MainScene.Ball
{
    public class SlowDownBall : MonoBehaviour
    {
        public SlownDownBallLogic SlowDownBallLogic { get; private set; }
        
        
        private void Awake() => SlowDownBallLogic = new SlownDownBallLogic();
    }
}