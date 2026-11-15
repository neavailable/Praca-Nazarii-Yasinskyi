using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Objects;


namespace MainScene.Enemy
{
    public class EnemyBounce : MonoBehaviour
    {
        private Bounce _bounce;


        [Inject]
        private void Constructor(EnemyBounceData enemyBounceData)
        {
            _bounce = new (gameObject, enemyBounceData.LostPower);
        }

        private void OnCollisionEnter(Collision collision)
        {
            _bounce.CollidedBall(collision);
        }
    }
}