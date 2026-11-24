namespace Configuration.Data
{
    public class ChaserData
    {
        public readonly float Speed, DetectionRadius, KnockPower, DamageCooldown;
        public readonly int Damage;


        public ChaserData
        (
            float speed,
            float detectionRadius,
            float knockPower,
            float damageCooldown,
            int damage
        )
        {
            Speed           = speed;
            DetectionRadius = detectionRadius;
            KnockPower      = knockPower;
            DamageCooldown  = damageCooldown;
            Damage          = damage;
        }
    }
}
