using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;
using MainScene.Ball.BallMarker;
using MainScene.Feedback;
using MainScene.Objects;


namespace MainScene.Chaser
{
    // The level 3 enemy: it rolls after the ball once the ball comes close, goes back home when it leaves,
    // and hurts and knocks the ball on contact. It cannot be defeated, only avoided.
    [RequireComponent(typeof(Rigidbody))]
    public class Chaser : MonoBehaviour
    {
        private const float _bodyRadius = 0.6f;

        private Rigidbody _rigidbody;
        private Transform _ball;
        private ChaserData _chaserData;
        private FeedbackPlayer _feedbackPlayer;
        private Vector3 _home;
        private float _nextHitTime;


        [Inject]
        private void Constructor(BallMarker ballMarker, ChaserData chaserData, FeedbackPlayer feedbackPlayer)
        {
            _ball           = ballMarker.transform;
            _chaserData     = chaserData;
            _feedbackPlayer = feedbackPlayer;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _home      = _rigidbody.position;
        }

        private void FixedUpdate()
        {
            Vector3 position = _rigidbody.position;
            Vector3 target   = IsBallNear(position) ? _ball.position : _home;

            Vector3 toTarget = Vector3.ProjectOnPlane(target - position, Vector3.up);
            if (toTarget.sqrMagnitude < 0.01f) return;

            Vector3 step = toTarget.normalized * Mathf.Min(_chaserData.Speed * Time.fixedDeltaTime, toTarget.magnitude);
            if (IsBlocked(position, step)) return;

            _rigidbody.MovePosition(position + step);
            _rigidbody.MoveRotation(Quaternion.LookRotation(step));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out BallMovement ballMovement)) return;
            if (Time.time < _nextHitTime) return;

            _nextHitTime = Time.time + _chaserData.DamageCooldown;

            Vector3 away = Vector3.ProjectOnPlane(ballMovement.transform.position - transform.position, Vector3.up);
            if (away.sqrMagnitude > 0.0001f) ballMovement.BallMovementLogic.Knock(away.normalized, _chaserData.KnockPower);
            if (ballMovement.TryGetComponent(out IGetDamage getDamage)) getDamage.GetDamage(_chaserData.Damage);

            _feedbackPlayer.Play(FeedbackType.Knock, collision.GetContact(0).point);
        }


        private bool IsBallNear(Vector3 position)
        {
            if (!_ball.gameObject.activeInHierarchy) return false;

            Vector3 toBall = Vector3.ProjectOnPlane(_ball.position - position, Vector3.up);
            return toBall.sqrMagnitude < _chaserData.DetectionRadius * _chaserData.DetectionRadius;
        }

        // A kinematic body would pass through walls, so it looks ahead and stops in front of anything but the ball.
        private bool IsBlocked(Vector3 position, Vector3 step)
        {
            bool hitSomething = Physics.SphereCast
            (
                position + Vector3.up * _bodyRadius,
                _bodyRadius * 0.9f,
                step.normalized,
                out RaycastHit hit,
                step.magnitude + 0.05f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );

            return hitSomething && hit.rigidbody != _rigidbody && !hit.collider.GetComponentInParent<BallMarker>();
        }
    }
}
