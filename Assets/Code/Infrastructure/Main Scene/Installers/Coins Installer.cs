using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Configuration.Data;
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
                    LoadCoinValue(),
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

        // Harder levels pay more for each coin.
        private CoinValueData LoadCoinValue()
        {
            int multiplier = new LevelRulesConfiguration(gameObject.scene.name).Load().CoinMultiplier;
            return new CoinValueData(new CoinValueConfiguration().Load().CoinValue * multiplier);
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}