using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.ClassesParameters;


namespace MainScene.Enemy
{
    public class CactusMovement : MonoBehaviour
    {
        public CactusMovementLogic CactusMovementLogic => _cactusMovementLogic;

        private Transform _target, _target2;
        private CactusMovementLogic _cactusMovementLogic;
        private float _speed;


        [Inject]
        private void Constructor
        (
            EnemyObject enemyObject, 
            CactusMovementData cactusMovementData
        )
        {
            _target  = enemyObject.Object.transform.GetChild(index: 0);
            _target2 = enemyObject.Object.transform.GetChild(index: 1);
            _speed   = cactusMovementData.Speed;
        }
        
        private void Awake()
        {
            _cactusMovementLogic = new
            (
                transform, 
                _target.position,
                _target2.position,
                GetComponentInChildren<EnemyDeathCollider>(),
                _speed
            );
        }
        
        private void FixedUpdate() => _cactusMovementLogic.Move();
        
        private void OnDestroy() => _cactusMovementLogic.Dispose();
    }
}