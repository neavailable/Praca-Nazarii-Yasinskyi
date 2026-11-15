using UnityEngine;
using Zenject;
using Configuration.Data;
using Configuration.Configurations;
using MainScene.Ball.BallMarker;


namespace Infrastructure.MainMenu
{
    public class BallMenuInstaller : MonoInstaller
    {
        [SerializeField] private Transform _ballStartPoint;
        [SerializeField] private BallMarker _ballMarker;


        public override void InstallBindings()
        {
            BindBall();
            InstantiateBall();
        }
        

        private void BindBall()
        {
            Container.Bind<BallMenuMovementData>()
                .FromInstance(new BallMenuMovementConfiguration().Load());
        }

        private void InstantiateBall()
        {
            Container.InstantiatePrefab
            (
                _ballMarker,
                _ballStartPoint.position,
                Quaternion.identity,
                null
            );
        }
    }
}