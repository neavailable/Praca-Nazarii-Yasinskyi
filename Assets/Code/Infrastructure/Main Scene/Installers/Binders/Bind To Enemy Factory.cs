using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;
using MainScene.Enemy;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToEnemyFactory : Binder
    {
        private readonly Stack<EnemyObject> _enemyTransforms;
        private readonly GameObject _cactusPrefab, _mushroomPrefab;
        
        
        public BindToEnemyFactory
        (
            DiContainer container, 
            AllEnemiesMarker allEnemiesMarker,
            GameObject cactusPrefab,
            GameObject mushroomPrefab
        ) : base(container)
        {
            _cactusPrefab    = cactusPrefab;
            _mushroomPrefab  = mushroomPrefab;
            
            _enemyTransforms = new ();
            FillEnemyTransforms(allEnemiesMarker);            
        }
        
        public override void Bind()
        {
            BindEnemyTransforms();
            BindCactusPrefab();
            BindMushroomPrefab();
        }


        private void FillEnemyTransforms(AllEnemiesMarker allEnemiesMarker)
        {
            foreach (Transform child in allEnemiesMarker.transform)
            {
                _enemyTransforms.Push(new EnemyObject(child.gameObject));
            }
        }

        private void BindEnemyTransforms()
        {
            Container.Bind<Stack<EnemyObject>>()
                .FromInstance(_enemyTransforms).AsSingle();
        }

        private void BindCactusPrefab()
        {
            Container.Bind<CactusPrefab>()
                .FromInstance(new CactusPrefab(_cactusPrefab)).AsSingle();
        }

        private void BindMushroomPrefab()
        {
            Container.Bind<MushroomPrefab>()
                .FromInstance(new MushroomPrefab(_mushroomPrefab)).AsSingle();
        }
    }
}