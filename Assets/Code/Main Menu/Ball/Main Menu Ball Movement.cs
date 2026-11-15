using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;


namespace MainMenu.Ball
{
    [RequireComponent(typeof(Rigidbody), typeof(BounceBall))]
    public class MainMenuBallMovement : MonoBehaviour, IBallMovementContainer
    {
        public AbstractBallMovementLogic BallMovementLogic { get; private set; }
        private float _pushingPower;

        
        [Inject]
        private void Constructor(BallMenuMovementData ballMovementData)
        {
            _pushingPower = ballMovementData.StartPushingPower;
        }
        
        private void Start()
        {
            BallMovementLogic = new MainMenuBallMovementLogic
            (
                GetComponent<Rigidbody>(),
                GetComponent<BounceBall>(),
                _pushingPower
            );
        }
        
        private void FixedUpdate() => BallMovementLogic.PushBall();
    }
}