using UnityEngine;


namespace MainScene.Enemy
{
    [RequireComponent(typeof(EnemyAnimations))]
    public class UnableEnemy : MonoBehaviour
    {
        private UnableEnemyLogic _unableEnemyLogic;
        
        
        private void OnEnable()
        {
            _unableEnemyLogic = new
            (
                gameObject,
                GetComponent<EnemyAnimations>().EnemyAnimationsLogic
            );
        }
        
        private void OnDisable() => _unableEnemyLogic.Dispose();
    }
}