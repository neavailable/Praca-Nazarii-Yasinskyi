using System;
using UnityEngine;
using AllScenes;


namespace MainScene.Ball
{
    public class BallRotationLogic : IDisposable
    {
        private readonly Transform _transform;
        private readonly Rigidbody _rigidbody;
        private readonly AbstractBallMovementLogic _abstractBallMovementLogic;
        private Vector3 _lastPosition;


        public BallRotationLogic
        (
            Transform transform,
            Rigidbody rigidbody,
            AbstractBallMovementLogic abstractBallMovementLogic
        )
        {
            _transform                 = transform;
            _rigidbody                 = rigidbody;
            _abstractBallMovementLogic = abstractBallMovementLogic;
            _lastPosition              = rigidbody.position;

            _abstractBallMovementLogic.BallPushed += RotateY;

            RotateY(_abstractBallMovementLogic.Direction);
        }

        // Rolls by how far the ball really moved, not by the velocity it is given:
        // a ball the wind presses against a wall stays still instead of spinning in place.
        public void RotateX()
        {
            Vector3 position = _rigidbody.position;
            Vector3 velocity = (position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = position;

            velocity.y = 0f;
            if (velocity.sqrMagnitude < 0.01f) return;

            float velocityX = Mathf.Abs(velocity.x);
            float velocityZ = Mathf.Abs(velocity.z);
            float rotateX = velocityX > velocityZ ? velocityX : velocityZ;

            _transform.Rotate
            (
                Vector3.Cross(Vector3.up, velocity.normalized),
                rotateX,
                Space.World
            );
        }
        
        public void Dispose() => _abstractBallMovementLogic.BallPushed -= RotateY;

        
        private void RotateY(Vector3 direction)
        {
            _transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}