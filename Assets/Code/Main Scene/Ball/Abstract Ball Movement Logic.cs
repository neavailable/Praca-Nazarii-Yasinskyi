using System;
using UnityEngine;
using R3;


namespace MainScene.Ball
{
    public abstract class AbstractBallMovementLogic : IDisposable
    {
        public readonly Subject<float> PushingPowerChanged;

        protected float PushingPower;

        public Vector3 Direction  { get; protected set; }
        public Vector3 Velocity => _rigidbody.velocity;
        public Vector3 Drift { get; set; }
        public bool IsRolling => PushingPower > 0.0f;
        public Action<Vector3> BallPushed;

        private readonly Rigidbody _rigidbody;
        private readonly BounceBall _bounceBall;


        protected AbstractBallMovementLogic
        (
            Rigidbody rigidbody,
            BounceBall bounceBall
        )
        {
            _rigidbody   = rigidbody;
            _bounceBall  = bounceBall;

            PushingPowerChanged = new ();

            _bounceBall.BounceBallLogic.GetBounceDirectionAction += BounceBall;
        }


        // Drift is added on top of the push and is not slowed by friction, so it also moves a ball at rest.
        public void PushBall() => _rigidbody.velocity = Direction * PushingPower + Drift;

        public void Launch(Vector3 direction, float power, float maxPushingPower)
        {
            Direction    = direction;
            PushingPower = Mathf.Min(PushingPower + power, maxPushingPower);

            PushingPowerChanged.OnNext(PushingPower);
            BallPushed?.Invoke(Direction);
        }

        // Unlike Launch it adds no speed: a fast ball keeps its speed, a slow one gets at least the given power.
        public void Knock(Vector3 direction, float power)
        {
            Direction    = direction;
            PushingPower = Mathf.Max(PushingPower, power);

            PushingPowerChanged.OnNext(PushingPower);
            BallPushed?.Invoke(Direction);
        }

        public virtual void Dispose()
        {
            _bounceBall.BounceBallLogic.GetBounceDirectionAction -= BounceBall;
        }


        private void BounceBall(Vector3 bounceDirection, float lostPower)
        {
            Direction = bounceDirection;
            PushingPower *= lostPower;

            PushingPowerChanged.OnNext(PushingPower);
            BallPushed?.Invoke(Direction);
        }
    }
}
