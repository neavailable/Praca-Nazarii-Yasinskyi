using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Objects;


namespace MainScene.Enemy
{
    public class MushroomDamageCollider : MonoBehaviour
    {
        private EnemyDamage _enemyDamage;
        
        
        [Inject]
        private void Constructor(MushroomDamageData mushroomDamageData)
        {
            _enemyDamage = new EnemyDamage(mushroomDamageData.Damage);
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