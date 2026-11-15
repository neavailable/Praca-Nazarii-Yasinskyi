using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.Enemy;


namespace Infrastructure.MainScene.Installers
{
    public class EnemiesInstaller : MonoInstaller
    {
        [SerializeField] private AllEnemiesMarker _allEnemiesMarker;
        [SerializeField] private CactusMarker _cactusMarker;
        [SerializeField] private MushroomMarker _mushroomMarker;
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
                new BindToEnemy
                (
                    Container,
                    new EnemyBounceConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindToCactus
                (
                    Container,
                    new CactusMovementConfiguration().Load(),
                    new CactusDamageConfiguration()  .Load()
                )
            );
            _binders.Add
            (
                new BindToMushroom
                (
                    Container,
                    new MushroomRotationConfiguration().Load(),
                    new MushroomDamageConfiguration()  .Load()
                )
            );
            _binders.Add
            (
                new BindToEnemyFactory
                (
                    Container,
                    _allEnemiesMarker,
                    _cactusMarker.gameObject,
                    _mushroomMarker.gameObject
                )
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}