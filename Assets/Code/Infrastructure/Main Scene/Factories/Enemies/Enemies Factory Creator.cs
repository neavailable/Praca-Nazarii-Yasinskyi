using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Factories.Enemies
{
    public class EnemiesFactoryCreator : MonoBehaviour
    {
        [Inject]
        private void Constructor
        (
            DiContainer diContainer,
            Stack<EnemyObject> enemyTransforms,
            CactusPrefab cactusPrefab,
            MushroomPrefab mushroomPrefab
        )
        {
             new EnemiesFactory
             (
                 diContainer,
                 cactusPrefab,
                 mushroomPrefab,
                 enemyTransforms
             );
        }
    }
}