using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToDustEffectsFactory : Binder
    {
        private readonly GameObject _dustEffectPrefab, _walls;

        
        public BindToDustEffectsFactory
        (
            DiContainer container,
            GameObject dustEffectPrefab,
            GameObject walls
        ) : base(container)
        {
            _dustEffectPrefab = dustEffectPrefab;
            _walls            = walls;
        }
        
        public override void Bind()
        {
            BindDustEffectPrefab();
            BindWalls();
        }


        private void BindDustEffectPrefab()
        {
            Container.Bind<DustEffectPrefab>().
                FromInstance(new DustEffectPrefab(_dustEffectPrefab)).AsSingle();
        }

        private void BindWalls()
        {
            Container.Bind<Walls>().
                FromInstance(new Walls(_walls)).AsSingle();
        }
    }
}