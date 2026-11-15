using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Obstacles
{
    // A bar turning around its post; each spinner picks its own turning side.
    [RequireComponent(typeof(Rigidbody))]
    public class Spinner : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private float _speed;


        [Inject]
        private void Constructor(ObstaclesData obstaclesData) => _speed = obstaclesData.SpinnerSpeed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (Random.value < 0.5f) _speed = -_speed;
        }

        private void FixedUpdate()
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0.0f, _speed * Time.fixedDeltaTime, 0.0f));
        }
    }
}
