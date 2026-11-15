using UnityEngine;


namespace MainScene.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimations : MonoBehaviour
    {
        public EnemyAnimationsLogic EnemyAnimationsLogic => _enemyAnimationsLogic;

        private EnemyAnimationsLogic _enemyAnimationsLogic;


        private void OnEnable()
        {
            _enemyAnimationsLogic = new
            (
                GetComponent<Animator>(),
                GetComponentInChildren<EnemyDeathCollider>()
            );
        }
        
        private void OnDisable() => _enemyAnimationsLogic.Dispose();
    }
}