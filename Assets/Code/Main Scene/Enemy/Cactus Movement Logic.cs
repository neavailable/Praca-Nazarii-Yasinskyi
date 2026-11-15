using System;
using UnityEngine;


namespace MainScene.Enemy
{
    public class CactusMovementLogic : IDisposable
    {
        public Action<Vector3> LookAt;
        
        private readonly Transform _transform;
        private readonly Vector3 _target, _target2;
        private readonly EnemyDeathCollider _enemyDeathCollider;
        private Vector3 _currentTarget;
        private float _speed;


        public CactusMovementLogic
        (
            Transform transform,
            Vector3 target, 
            Vector3 target2,
            EnemyDeathCollider enemyDeathCollider,
            float speed
        )
        {
            _transform          = transform;
            _target             = target;
            _target2            = target2;
            _enemyDeathCollider = enemyDeathCollider;
            _currentTarget      = _target;
            _speed              = speed;

            _enemyDeathCollider.BallPushed += Stop;
        }
        
        public void Move()
        {
            if (_speed == 0) return;
            
            _transform.position = Vector3.MoveTowards
            (
                _transform.position, 
                _currentTarget, 
                _speed * Time.fixedDeltaTime
            );
            
            if (_transform.position == _currentTarget) SetNewTarget();
            LookAt?.Invoke(_currentTarget);
        }
        
        public void Dispose() => _enemyDeathCollider.BallPushed -= Stop;
        
        
        private void Stop() => _speed = 0;
        
        private void SetNewTarget()
        {
            _currentTarget = _currentTarget == _target ? _target2 : _target;
        }
    }
}