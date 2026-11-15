using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using Infrastructure.MainScene.Factories.Ball;
using MainScene.ClassesParameters;
using MainScene.UserInput;
using MainScene.Ball.BallMarker;
using MainScene.Finish.FinishTrigger;


namespace Infrastructure.MainScene.Installers
{
    public class BallInstaller : MonoInstaller
    {
        [SerializeField] private Transform _ballStartPoint;
        [SerializeField] private BallInput _ballInput;
        [SerializeField] private FinishTrigger _finishTrigger;
        [SerializeField] private BallMarker _ballMarker;
        private readonly List<Binder> _toBallBinders = new ();
        private readonly List<Binder> _fromBallBinders = new ();

        
        public override void InstallBindings()
        {
            CreateToBallBinders();
            BindToBall();
            CreateFromBallBinders();
            BindFromBall();
        }
        

        private void CreateToBallBinders()
        {
            _toBallBinders.Add
            (
                new BindToBall
                (
                    Container,
                    _ballInput,
                    _finishTrigger,
                    new BallMovementConfiguration().Load(),
                    new BallBounceAnimationConfiguration().Load(),
                    new HealthConfiguration().Load()
                )
            );
            _toBallBinders.Add
            (
                new BindToWind(Container, new WindConfiguration().Load())
            );
        }

        private void CreateFromBallBinders()
        {
            BallFactory ballFactory = new
            (
                Container,
                new BallPrefab(_ballMarker.gameObject),
                _ballStartPoint.position,
                Quaternion.identity
            );
            
            _fromBallBinders.Add
            (
                new BindFromBall
                (
                    Container, 
                    ballFactory.InstantiateBall()
                )
            );
        }

        private void BindToBall()
        {
            foreach (Binder binder in _toBallBinders) binder.Bind();
        }
        
        private void BindFromBall()
        {
            foreach (Binder binder in _fromBallBinders) binder.Bind();
        }
    }
}