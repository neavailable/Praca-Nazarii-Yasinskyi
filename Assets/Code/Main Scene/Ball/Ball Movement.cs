using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.UserInput;


namespace MainScene.Ball
{
    [RequireComponent(typeof(Rigidbody), typeof(SlowDownBall), typeof(BounceBall))]
    public class BallMovement : MonoBehaviour, IBallMovementContainer
    {
        public AbstractBallMovementLogic BallMovementLogic { get; private set; }
        
        private BallInput _ballInput;
        private BallMovementData _ballMovementData;
        
        
        [Inject]
        private void Constructor(BallMovementData ballMovementData, BallInput ballInput)
        {
            _ballMovementData = ballMovementData;
            _ballInput        = ballInput;
        }
        
        private void Start()
        {
            BallMovementLogic = new BallMovementLogic
            (
                GetComponent<Rigidbody>(),
                transform,
                _ballInput,
                GetComponent<BounceBall>(),
                GetComponent<SlowDownBall>(),
                _ballMovementData.MaxPushingPower,
                _ballMovementData.PushPowerDecrease
            );
        }

        private void FixedUpdate() => BallMovementLogic.PushBall();

        private void OnDisable() => BallMovementLogic.Dispose();
    }
}