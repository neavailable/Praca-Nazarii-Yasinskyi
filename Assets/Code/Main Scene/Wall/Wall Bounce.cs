using System;
using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Objects;


namespace MainScene.Wall
{
    public class WallBounce : MonoBehaviour
    {
        public Action<Vector3, Vector3> BouncedAction;

        private Bounce _bounce;


        [Inject]
        private void Constructor(WallData wallData)
        {
            _bounce = new (gameObject, wallData.LostPower);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var (point, normal) = _bounce.CollidedBall(collision);
            
            if (point == Vector3.positiveInfinity && normal == Vector3.negativeInfinity) return;
            
            BouncedAction?.Invoke(point, normal);
        }
    }
}