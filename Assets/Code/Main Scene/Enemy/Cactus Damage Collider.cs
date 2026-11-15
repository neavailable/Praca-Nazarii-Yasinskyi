using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Objects;


namespace MainScene.Enemy
{
    public class CactusDamageCollider : MonoBehaviour
    {
        private EnemyDamage _enemyDamage;
        
        
        [Inject]
        private void Constructor(CactusDamageData cactusDamageData)
        {
            _enemyDamage = new EnemyDamage(cactusDamageData.Damage);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IGetDamage obj))
            {
                obj.GetDamage(_enemyDamage.Damage);
            }
        }
    }
}