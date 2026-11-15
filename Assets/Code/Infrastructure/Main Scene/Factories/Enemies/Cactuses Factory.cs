using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;
using MainScene.Enemy;


namespace Infrastructure.MainScene.Factories.Enemies
{
    public class CactusesFactory 
    {
        private readonly Stack<GameObject> _cactuses;
        private readonly GameObject _cactusPrefab;
        private readonly DiContainer _diContainer;
        
        
        public CactusesFactory
        (
            DiContainer diContainer, 
            AbstractGameObjectParametr objectPrefab,
            Stack<EnemyObject> enemyTransforms
        )
        {
            _cactusPrefab = objectPrefab.Object;
            _diContainer  = diContainer;
            
            _cactuses = new ();
            
            FillCactuses(enemyTransforms);
        }
        

        private void FillCactuses(Stack<EnemyObject> cactusTransforms)
        {
            foreach (EnemyObject enemyTransform in cactusTransforms)
            {
                _cactuses.Push(InstantiateCactus(enemyTransform));
            }
        }
        
        private GameObject InstantiateCactus(EnemyObject cactusObject)
        {
            return _diContainer.InstantiatePrefabForComponent<CactusMovement>
            (
                _cactusPrefab,
                cactusObject.Object.transform.position,
                Quaternion.identity,
                null,
                new [] { cactusObject }
            ).gameObject;
        }
    }
}