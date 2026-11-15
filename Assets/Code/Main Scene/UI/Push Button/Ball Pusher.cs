using UnityEngine;
using MainScene.Ball;


namespace MainScene.UI.Push
{
    // Gives a ball at rest a light push in a random direction, e.g. when it got stuck where it cannot be clicked.
    public class BallPusher
    {
        public bool CanPush =>
            _ballMovement.gameObject.activeInHierarchy &&
            _ballMovement.BallMovementLogic is { IsRolling: false };

        private readonly BallMovement _ballMovement;
        private readonly float _pushPower;


        public BallPusher(BallMovement ballMovement, float pushPower)
        {
            _ballMovement = ballMovement;
            _pushPower    = pushPower;
        }

        public void Push()
        {
            if (!CanPush) return;

            Vector3 direction = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Vector3.forward;
            _ballMovement.BallMovementLogic.Launch(direction, _pushPower, _pushPower);
        }
    }
}
