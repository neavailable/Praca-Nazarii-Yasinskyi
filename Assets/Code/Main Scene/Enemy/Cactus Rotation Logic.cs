using UnityEngine;
using System;


namespace MainScene.Enemy
{
    public class CactusRotationLogic : IDisposable
    {
        private readonly Transform _transform;
        private readonly CactusMovementLogic _cactusMovementLogic;
        
        
        public CactusRotationLogic(Transform transform, CactusMovementLogic cactusMovementLogic)
        {
            _transform          = transform;
            _cactusMovementLogic = cactusMovementLogic;

            _cactusMovementLogic.LookAt += LookAt;
        }

        public void Dispose() => _cactusMovementLogic.LookAt -= LookAt;


        private void LookAt(Vector3 target) => _transform.LookAt(target);
    }
}