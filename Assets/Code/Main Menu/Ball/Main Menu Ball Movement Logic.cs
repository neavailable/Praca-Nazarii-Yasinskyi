using UnityEngine;
using MainScene.Ball;


namespace MainMenu.Ball
{
    public class MainMenuBallMovementLogic : AbstractBallMovementLogic
    {
        private const float _minMaxRandomDirection = 1.0f;

        
        public MainMenuBallMovementLogic
        (
            Rigidbody rigidbody,
            BounceBall bounceBall,
            float startPushingPower
        ) : base(rigidbody, bounceBall)
        {
            PushingPower = startPushingPower;
            
            Direction = new Vector3
            (
                Random.Range(-_minMaxRandomDirection, _minMaxRandomDirection),
                0.0f,                     
                Random.Range(-_minMaxRandomDirection, _minMaxRandomDirection)
            ).normalized;
        }
    }
}