using System;
using UnityEngine;


namespace MainScene.Ball
{
    public class BounceBallLogic
    {
        public Action<Vector3, float> GetBounceDirectionAction;
        public Action<GameObject> BallBounced;

        private readonly IBallMovementContainer _ballMovement;


        public BounceBallLogic(IBallMovementContainer ballMovement)
        {
            _ballMovement = ballMovement;
        }

        public void GetBounceDirection
        (
            GameObject collidedObject,
            Vector3 normal,
            float lostPower
        )
        {
            GetBounceDirectionAction?.Invoke
            (
                Vector3.Reflect
                (
                    _ballMovement.BallMovementLogic.Direction, 
                    normal
                ),
                lostPower
            );
            BallBounced?.Invoke(collidedObject);
        }
    }
}