using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Obstacles
{
    // Slides back and forth along its own length, slowing down at both ends.
    [RequireComponent(typeof(Rigidbody))]
    public class MovingWall : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Vector3 _center, _axis;
        private float _speed, _distance, _phase;


        [Inject]
        private void Constructor(ObstaclesData obstaclesData)
        {
            _speed    = obstaclesData.MovingWallSpeed;
            _distance = obstaclesData.MovingWallDistance;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _center    = _rigidbody.position;
            _axis      = transform.right;
            _phase     = Random.Range(0.0f, 2.0f * Mathf.PI);
        }

        private void FixedUpdate()
        {
            float offset = Mathf.Sin(Time.time * _speed / _distance + _phase) * _distance;
            _rigidbody.MovePosition(_center + _axis * offset);
        }
    }
}
