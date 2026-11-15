using System;


namespace MainScene.UI.HealthBar
{
    public class HealthStorage
    {
        private const int _minHealth = 0;

        public Action Die;
        public readonly int DefaultHealth;
        public int Health { get; private set; }

        
        public HealthStorage(int health)
        {
            Health        = health;
            DefaultHealth = Health;
        }

        public void DecreaseHealth(int health)
        {
            Health -= health;

            if (Health < _minHealth)
            {
                Health = _minHealth;
                Die?.Invoke();
            }
        }

        public bool IncreaseHealth(int health)
        {
            if (Health >= DefaultHealth) return false;

            Health = Math.Min(Health + health, DefaultHealth);
            return true;
        }
    }
}