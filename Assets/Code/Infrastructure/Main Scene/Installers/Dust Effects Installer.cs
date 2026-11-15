using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.DustEffect;
using MainScene.Wall.WallsMarker;


namespace Infrastructure.MainScene.Installers
{
    public class DustEffectsInstaller : MonoInstaller
    {
        [SerializeField] private DustEffectMarker _dustEffectPrefab;
        [SerializeField] private AllWallsMarker _wall;
        private readonly List<Binder> _binders = new ();


        public override void InstallBindings()
        {
            CreateBinders();
            Bind();
        }
        

        private void CreateBinders()
        {
            _binders.Add
            (
                new BindToDustEffect
                (
                    Container,
                    new DustEffectConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindToDustEffectsFactory
                (
                    Container,
                    _dustEffectPrefab.gameObject,
                    _wall.gameObject
                )
            );
        }
        
        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}