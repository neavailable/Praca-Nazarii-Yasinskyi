using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Factories.Enemies
{
    public class MushroomsFactory
    {
        private readonly GameObject _mushroomPrefab;
        private readonly DiContainer _diContainer;
        
        
        public MushroomsFactory
        (
            DiContainer diContainer, 
            AbstractGameObjectParametr objectPrefab,
            Stack<EnemyObject> enemyTransforms
        )
        {
            _mushroomPrefab = objectPrefab.Object;
            _diContainer    = diContainer;
            
            FillMushrooms(enemyTransforms);
        }
        

        private void FillMushrooms(Stack<EnemyObject> mushroomsTransforms)
        {
            foreach (EnemyObject enemyTransform in mushroomsTransforms)
            {
                InstantiateMushroom(enemyTransform);
            }
        }

        private void InstantiateMushroom(EnemyObject enemyObject)
        {
            _diContainer.InstantiatePrefab
            (
                _mushroomPrefab,
                enemyObject.Object.transform.position,
                Quaternion.identity,
                null
            );
        }
    }
}