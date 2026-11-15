using System;
using UnityEngine;
using MainScene.Ball.BallMarker;


namespace MainScene.Enemy
{
    public class EnemyDeathCollider : MonoBehaviour
    {
        public Action BallPushed;


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BallMarker _))
            {
                BallPushed?.Invoke();
            }
        }
    }
}