using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.Coin;


namespace Infrastructure.MainScene.Installers
{
    public class CoinsInstaller : MonoInstaller
    {
        [SerializeField] private AllCoinsMarker _allCoinsMarker;
        [SerializeField] private CoinMarker _coinMarker;
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
                new BindToCoins
                (
                    Container,
                    new CoinValueConfiguration().Load(),
                    new CoinMovementConfiguration().Load(),
                    new CoinRotationConfiguration().Load(),
                    new CoinDestroyerConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindToCoinsFactory
                (
                    Container,
                    _allCoinsMarker,
                    _coinMarker.gameObject
                )
            );
            _binders.Add
            (
                new BindToHeal(Container, new HealConfiguration().Load())
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}