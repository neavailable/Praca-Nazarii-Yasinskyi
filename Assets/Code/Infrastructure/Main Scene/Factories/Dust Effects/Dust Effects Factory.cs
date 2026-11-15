using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;
using MainScene.DustEffect;
using MainScene.Wall;


namespace Infrastructure.MainScene.Factories.DustEffects
{
    public class DustEffectsFactory : IDisposable
    {
        private const int _maxNumberOfDustEffects = 7;
        private readonly GameObject _coinPrefab;
        private readonly DiContainer _diContainer;
        private readonly Stack<DustEffect> _dustEffects;
        private readonly Stack<WallBounce> _bounces;

        
        public DustEffectsFactory
        (
            DiContainer diContainer,
            AbstractGameObjectParametr objectPrefab, 
            AbstractGameObjectParametr walls
        )
        {
            _coinPrefab  = objectPrefab.Object;
            _diContainer = diContainer;
            
            _dustEffects = new Stack<DustEffect>();
            _bounces     = new Stack<WallBounce>();

            FillBouncesList(walls);
            FillDustEffectsStack();
        }

        public void Dispose()
        {
            while (!_bounces.IsEmpty()) _bounces.Pop().BouncedAction -= Pop;

            while (!_dustEffects.IsEmpty()) _dustEffects.Pop().DustEffectStopped -= PushDustEffect;
        }
        
        
        private void Pop(Vector3 position, Vector3 normal)
        {
            DustEffect dustEffect = _dustEffects.Pop();
            
            dustEffect.gameObject.SetActive(true);

            dustEffect.transform.position = position;
            dustEffect.transform.rotation = Quaternion.LookRotation(normal);
        }
        
        private DustEffect InstantiateDustEffect()
        {
            DustEffect dustEffect = _diContainer.InstantiatePrefabForComponent<DustEffect>
            (
                _coinPrefab,
                Vector3.zero,
                Quaternion.identity,
                null
            );
            
            dustEffect.DustEffectStopped += PushDustEffect; 
            dustEffect.gameObject.SetActive(false);
            
            return dustEffect;
        }
        
        private void PushDustEffect(DustEffect dustEffect)
        {
            _dustEffects.Push(dustEffect);
            
            dustEffect.gameObject.SetActive(true);
            
            dustEffect.transform.position = Vector3.zero;
            dustEffect.transform.rotation = Quaternion.identity;
        }
        
        
        private void FillBouncesList(AbstractGameObjectParametr walls)
        {
            foreach (Transform wall in walls.Object.transform)
            {
                WallBounce wallBounce =  wall.GetComponent<WallBounce>();
                wallBounce.BouncedAction += Pop; 
                _bounces.Push(wallBounce);
            }
        }

        private void FillDustEffectsStack()
        {
            for (int i = 0; i < _maxNumberOfDustEffects; ++i) _dustEffects.Push(InstantiateDustEffect());
        }
    }
}