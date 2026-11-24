using System;
using UnityEngine;
using Zenject;
using R3;
using Configuration.Data;
using MainScene.Ball;
using MainScene.Objects;


namespace MainScene.UI.HealthBar.MVVM
{
    [RequireComponent(typeof(BallGetDamage))]
    public class HealthBarModel : MonoBehaviour, IGetHeal
    {
        public event Action<int> HealthChanged;
        public Subject<Unit> Die;
        public int DefaultHealth => _healthStorage.DefaultHealth;

        private HealthStorage _healthStorage;
        private IDisposable _getDamage;


        [Inject]
        private void Constructor(UIHealthData UIHealthData)
        {
            Die = new();
            _healthStorage = new (UIHealthData.DefaultHealth);
            
            _healthStorage.Die += () => Die?.OnNext(Unit.Default);
        }
        private void Start()
        {
            _getDamage = GetComponent<BallGetDamage>()
                .SendDamage.Subscribe(GetDamage);
        }
        
        public bool TryHeal(int heal)
        {
            if (!_healthStorage.IncreaseHealth(heal)) return false;

            HealthChanged?.Invoke(_healthStorage.Health);
            return true;
        }

        // Used when the level time runs out: the ball loses all its health.
        public void Kill() => GetDamage(_healthStorage.Health + 1);

        private void GetDamage(int damage)
        {
            _healthStorage.DecreaseHealth(damage);
            HealthChanged?.Invoke(_healthStorage.Health);
        }
        
        private void OnDestroy()
        {
            _getDamage.Dispose();
            _healthStorage.Die -= () => Die?.OnNext(Unit.Default);
        }
    }
}