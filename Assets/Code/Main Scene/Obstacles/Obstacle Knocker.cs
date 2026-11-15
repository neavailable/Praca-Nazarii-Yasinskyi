using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;


namespace MainScene.Obstacles
{
    // A moving obstacle sends the ball away from the touched surface, so the ball is never dragged along or pinned.
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleKnocker : MonoBehaviour
    {
        private float _knockPower;


        [Inject]
        private void Constructor(ObstaclesData obstaclesData) => _knockPower = obstaclesData.KnockPower;

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out BallMovement ballMovement)) return;

            Vector3 ballPosition = ballMovement.transform.position;
            Vector3 direction = Vector3.ProjectOnPlane(ballPosition - collision.GetContact(0).point, Vector3.up);
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector3.ProjectOnPlane(ballPosition - transform.position, Vector3.up);
            }

            ballMovement.BallMovementLogic.Knock(direction.normalized, _knockPower);
        }
    }
}
