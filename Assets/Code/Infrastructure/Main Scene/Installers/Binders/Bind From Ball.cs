using UnityEngine;
using Zenject;
using MainScene.Ball.BallMarker;
using MainScene.Ball.BallColliders;
using MainScene.Ball.BallDestroyer;
using MainScene.Ball.UIDisplayable;
using MainScene.UI.HealthBar.MVVM;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindFromBall : Binder
    {
        private readonly GameObject _ball;


        public BindFromBall(DiContainer container, GameObject ball)
            : base(container)
        {
            _ball = ball;
        }

        public override void Bind()
        {
            BindBallMarker();
            BindBallTrigger();
            BindBallMeshRotation();
            BindBallSpeedSender();
            BindHealthBarModel();
            BindBallDestroyer();
        }
        

        private void BindBallMarker()
        {
            Container
                .Bind<BallMarker>()
                .FromInstance(_ball.GetComponent<BallMarker>())
                .AsSingle();   
        }
        
        private void BindBallTrigger()
        {
            Container
                .Bind<BallTrigger>()
                .FromInstance(_ball.GetComponent<BallTrigger>())
                .AsSingle();   
        }

        private void BindBallMeshRotation()
        {
            Container
                .Bind<BallMeshRotationSender>()
                .FromInstance(_ball.GetComponentInChildren<BallMeshRotationSender>())
                .AsSingle();   
        }

        private void BindBallSpeedSender()
        {
            Container
                .Bind<BallSpeedSender>()
                .FromInstance(_ball.GetComponent<BallSpeedSender>())
                .AsSingle();   
        }

        private void BindHealthBarModel()
        {
            Container
                .Bind<HealthBarModel>().
                FromInstance(_ball.GetComponent<HealthBarModel>()).
                AsSingle();
        }   
        
        private void BindBallDestroyer()
        {
            Container
                .Bind<BallDestroyer>().
                FromInstance(_ball.GetComponent<BallDestroyer>()).
                AsSingle();
        }   
    }
}