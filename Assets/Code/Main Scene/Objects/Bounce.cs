using UnityEngine;
using MainScene.Ball;


namespace MainScene.Objects
{
    public class Bounce
    {
        private readonly GameObject _gameObject;
        private readonly float _lostPower;


        public Bounce(GameObject gameObject, float lostPower)
        {
            _lostPower  = lostPower;
            _gameObject = gameObject;
        }


        public (Vector3, Vector3) CollidedBall(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BounceBall bounceBall))
            {
                ContactPoint contactPoint = collision.contacts[0];
                bounceBall.BounceBallLogic.GetBounceDirection
                (
                    _gameObject,
                    contactPoint.normal, 
                    _lostPower
                );
                return (contactPoint.point, contactPoint.normal);
            }
            
            return (Vector3.positiveInfinity, Vector3.positiveInfinity);
        }
    }
}