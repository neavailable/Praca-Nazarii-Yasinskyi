using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;


namespace MainScene.Obstacles
{
    // A pinball bumper: throws the ball away from its centre faster than it came in, and pops a little.
    public class Bumper : MonoBehaviour
    {
        private const float _popDuration = 0.15f, _popScale = 0.25f;

        [SerializeField] private Transform _body;

        private ObstaclesData _obstaclesData;
        private Vector3 _bodyScale;
        private float _popTime;


        [Inject]
        private void Constructor(ObstaclesData obstaclesData) => _obstaclesData = obstaclesData;

        private void Awake() => _bodyScale = _body.localScale;

        private void Update()
        {
            if (_popTime <= 0.0f) return;

            _popTime = Mathf.Max(_popTime - Time.deltaTime, 0.0f);
            _body.localScale = _bodyScale * (1.0f + _popScale * _popTime / _popDuration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out BallMovement ballMovement)) return;

            Vector3 direction = Vector3.ProjectOnPlane(ballMovement.transform.position - transform.position, Vector3.up);
            if (direction.sqrMagnitude < 0.0001f) return;

            ballMovement.BallMovementLogic.Launch
            (
                direction.normalized,
                _obstaclesData.BumperPower,
                _obstaclesData.BumperMaxSpeed
            );
            _popTime = _popDuration;
        }
    }
}
