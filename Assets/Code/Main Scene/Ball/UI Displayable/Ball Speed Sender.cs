using System;
using UnityEngine;
using R3;


namespace MainScene.Ball.UIDisplayable
{
    [RequireComponent(typeof(BallMovement))]
    public class BallSpeedSender : MonoBehaviour
    {
        public ReactiveProperty<float> Speed;

        private IDisposable _speedChanged;
        

        private void Awake() => Speed = new ();

        private void Start()
        {
            _speedChanged = GetComponent<BallMovement>().BallMovementLogic
                .PushingPowerChanged.Subscribe(ChangeSpeed);
        }

        private void ChangeSpeed(float pushingPower) => Speed.Value = pushingPower;
        
        private void OnDestroy() => _speedChanged.Dispose();
    }
}