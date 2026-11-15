using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Factories.Ball
{
    public class BallFactory
    {
        private readonly GameObject _ballPrefab;
        private readonly DiContainer _diContainer;
        private readonly Vector3 _ballPosition;
        private readonly Quaternion _ballRotation;
        
        
        public BallFactory
        (
            DiContainer diContainer, 
            AbstractGameObjectParametr objectPrefab,
            Vector3 ballPosition,
            Quaternion ballRotation
        )
        {
            _ballPrefab = objectPrefab.Object;
            _diContainer = diContainer;
            _ballPosition = ballPosition;
            _ballRotation = ballRotation;
        }
        
        public GameObject InstantiateBall()
        {
            return _diContainer.InstantiatePrefab
            (
                _ballPrefab,
                _ballPosition,
                _ballRotation,
                null
            );
        }
    }
}