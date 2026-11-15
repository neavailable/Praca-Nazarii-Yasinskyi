using UnityEngine;


namespace MainScene.Coin
{
    public class CoinMovementLogic
    {
        private readonly Transform _transform;
        private readonly float _ySpeed;
        private Vector3 _target;
        private float _yBorder;


        public CoinMovementLogic(Transform transform, float ySpeed, float yBorder)
        {
            _transform = transform;
            _ySpeed    = ySpeed;
            _yBorder   = yBorder;
            
            SetTarget();
        }
        
        
        private void SetTarget()
        {
            _target = _transform.position;
            _target += _yBorder * Vector3.up;
        }

        private void SetNewTarget()
        {
            _yBorder = -_yBorder;
            
            SetTarget();
        }
        
        
        public void Move()
        {
            _transform.position = Vector3.MoveTowards
            (
                _transform.position,
                _target,
                _ySpeed * Time.fixedDeltaTime
            );

            if (_transform.position == _target) SetNewTarget();
        }
    }
}