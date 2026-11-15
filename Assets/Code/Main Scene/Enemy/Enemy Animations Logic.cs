using System;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace MainScene.Enemy
{
    public class EnemyAnimationsLogic : IDisposable
    {
        public Action DieAnimationFinished;
        
        private const string _dyingParametrName = "Is Dying";
        private const float _dyingAnimationDuration = 1.7f;
        private readonly Animator _animator;
        private readonly EnemyDeathCollider _enemyDeathCollider;


        public EnemyAnimationsLogic
        (
            Animator animator, 
            EnemyDeathCollider enemyDeathCollider
        ) 
        {
            _animator               = animator;
            _enemyDeathCollider     = enemyDeathCollider;
            
            _enemyDeathCollider.BallPushed += StartDyingAnimation;
        }

        public void Dispose() => _enemyDeathCollider.BallPushed -= StartDyingAnimation;


        private async UniTaskVoid Die()
        {
            await UniTask.WaitForSeconds(_dyingAnimationDuration);
            
            DieAnimationFinished?.Invoke();
        }
        
        private void StartDyingAnimation()
        {
            _animator.SetBool(_dyingParametrName, true);
            
            Die().Forget();
        }
    }
}