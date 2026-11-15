namespace Configuration.Data
{
    public class ObstaclesData
    {
        public readonly float BumperPower, BumperMaxSpeed, KnockPower;
        public readonly float SpinnerSpeed, MovingWallSpeed, MovingWallDistance;


        public ObstaclesData
        (
            float bumperPower,
            float bumperMaxSpeed,
            float knockPower,
            float spinnerSpeed,
            float movingWallSpeed,
            float movingWallDistance
        )
        {
            BumperPower        = bumperPower;
            BumperMaxSpeed     = bumperMaxSpeed;
            KnockPower         = knockPower;
            SpinnerSpeed       = spinnerSpeed;
            MovingWallSpeed    = movingWallSpeed;
            MovingWallDistance = movingWallDistance;
        }
    }
}
