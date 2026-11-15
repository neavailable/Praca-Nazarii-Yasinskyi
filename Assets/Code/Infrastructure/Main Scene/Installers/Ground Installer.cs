using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.Ground;


namespace Infrastructure.MainScene.Installers
{
    public class GroundInstaller : MonoInstaller
    {
        [SerializeField] private GroundMarker _groundMarker;
        private readonly List<Binder> _binders = new ();

        
        public override void InstallBindings()
        {
            CreteBinders();
            Bind();
        }
        

        private void CreteBinders()
        {
            _binders.Add
            (
                new BindToGround
                (
                    Container,
                    new GroundConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindFromGround
                (
                    Container,
                    _groundMarker
                )
            );
            _binders.Add
            (
                new BindToBoostPad
                (
                    Container,
                    new BoostPadConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindToObstacles
                (
                    Container,
                    new ObstaclesConfiguration().Load()
                )
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}