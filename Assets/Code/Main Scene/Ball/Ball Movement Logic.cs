using UnityEngine;
using MainScene.UserInput;
using MainScene.RayPoints;
using MainScene.RayPoints.GroundPoint;
using MainScene.RayPoints.ScreenPoint;


namespace MainScene.Ball
{
    public class BallMovementLogic : AbstractBallMovementLogic
    {
        private readonly Transform _transform;
        private readonly BallInput _ballInput;
        private readonly SlowDownBall _slowDownBall;
        private readonly PushPowerConverter _pushPowerConverter;
        private readonly IGetRayPoint _screenRay, _groundRay;

        
        public BallMovementLogic
        (
            Rigidbody rigidbody,
            Transform transform,
            BallInput ballInput,
            BounceBall bounceBall,
            SlowDownBall slowDownBall,
            float maxPushingPower,
            float pushPowerDecrease
        ) : base(rigidbody, bounceBall)
        {
            _transform    = transform;
            _ballInput    = ballInput;
            _slowDownBall = slowDownBall;
            
            _pushPowerConverter = new PushPowerConverter
            (
                maxPushingPower, 
                pushPowerDecrease
            );
            _screenRay          = new ScreenPoint();
            _groundRay          = new GroundPoint();
            
            _ballInput   .BallInputLogic   .LeftMouseButtonPressedAction  += WriteStartPoint;
            _ballInput   .BallInputLogic   .LeftMouseButtonReleasedAction += StartPushBall;
            _slowDownBall.SlowDownBallLogic.GetSlowedDirectionAction      += SlowDownBall;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            _ballInput   .BallInputLogic   .LeftMouseButtonPressedAction  -= WriteStartPoint;
            _ballInput   .BallInputLogic   .LeftMouseButtonReleasedAction -= StartPushBall;
            _slowDownBall.SlowDownBallLogic.GetSlowedDirectionAction      -= SlowDownBall;
        }

        
        private void WriteStartPoint()
        {
            _pushPowerConverter.ScreenStartPoint = _screenRay.GetPoint();
        }
        
        private void StartPushBall()
        {
            Vector3 ballPosition = _transform.position;
            ballPosition.y = 0.0f;
            
            _pushPowerConverter.GroundStartPoint  = ballPosition;
            _pushPowerConverter.ScreenFinishPoint = _screenRay.GetPoint();
            _pushPowerConverter.GroundFinishPoint = _groundRay.GetPoint();  
            
            (PushingPower, Direction) = _pushPowerConverter.ConvertToPushPower();
            
            BallPushed?.Invoke(Direction);
        }

        private void SlowDownBall(float frictionPower)
        {
            if (PushingPower <= 0.0f)
            {
                PushingPower = 0.0f;
                PushingPowerChanged.OnNext(PushingPower);
                
                return;
            }

            PushingPower = Mathf.Max(PushingPower - frictionPower, 0.0f);
            PushingPowerChanged.OnNext(PushingPower);
        }
    }
}