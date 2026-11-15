using UnityEngine;


namespace MainScene.Ball.BounceAnimation
{
    public class BallExtendingAnimation : IBallAnimation
    {
        private readonly BallMovement _ballMovement;
        private readonly Transform _transform;
        private readonly Vector3 _defaultScale;
        private readonly float _extendingSpeedDivider;
        

        public BallExtendingAnimation 
        (
            BallMovement ballMovement,
            Transform transform,
            float extendingSpeedDivider
        ) 
        {
            _ballMovement          = ballMovement;
            _transform             = transform;
            _extendingSpeedDivider = extendingSpeedDivider;
            _defaultScale          = _transform.localScale;
        }
        
        public void PlayAnimation()
        {
            if (_ballMovement is null) return;
            
            float xAnimationSpeed = _ballMovement.BallMovementLogic.Velocity.magnitude 
                                    / _extendingSpeedDivider;
            float zAnimationSpeed = _ballMovement.BallMovementLogic.Velocity.magnitude
                                    / _extendingSpeedDivider;

            _transform.localScale = new Vector3
            (
                x: _transform.localScale.x - xAnimationSpeed * Time.fixedDeltaTime,
                y: _defaultScale.y,
                z: _transform.localScale.z + zAnimationSpeed * Time.fixedDeltaTime
            );
        }

        public void FinishBouncingAnimation() => _transform.localScale = _defaultScale;
    }
}