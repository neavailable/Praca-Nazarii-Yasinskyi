using UnityEngine;


namespace MainScene.Ball.BounceAnimation
{
    public class BallSqueezingAnimation : IBallAnimation
    {
        private readonly BallMovement _ballMovement;
        private readonly Transform _transform;
        private readonly Vector3 _defaultScale;
        private readonly float _squeezingSpeedDivider;
        

        public BallSqueezingAnimation 
        (
            BallMovement ballMovement,
            Transform transform,
            float squeezingSpeedDivider
        ) 
        {
            _ballMovement          = ballMovement;
            _transform             = transform;
            _squeezingSpeedDivider = squeezingSpeedDivider;
            _defaultScale          = _transform.localScale;
        }
        
        public void PlayAnimation()
        {
            if (_ballMovement.BallMovementLogic is null) return;

            float xAnimationSpeed = _ballMovement.BallMovementLogic.Velocity.magnitude
                                    / _squeezingSpeedDivider;
            float zAnimationSpeed = _ballMovement.BallMovementLogic.Velocity.magnitude 
                                    / _squeezingSpeedDivider;

            _transform.localScale = new Vector3
            (
                x: _transform.localScale.x + xAnimationSpeed * Time.fixedDeltaTime,
                y: _defaultScale.y,
                z: _transform.localScale.z - zAnimationSpeed * Time.fixedDeltaTime
            );
        }
        
        public void FinishBouncingAnimation() { }
    }
}