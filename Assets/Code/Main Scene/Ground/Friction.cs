using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;


namespace MainScene.Ground
{
    public class Friction : MonoBehaviour
    {
        private float _frictionPower;


        [Inject]
        private void Constructor(GroundData groundData) => _frictionPower = groundData.FrictionPower;
        
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out SlowDownBall slowDownBall))
            {
                slowDownBall.SlowDownBallLogic.GetSlowedDirection(_frictionPower * Time.fixedDeltaTime);
            }
        }
    }
}