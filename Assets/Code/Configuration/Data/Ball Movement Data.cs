namespace Configuration.Data
{
    public class BallMovementData
    {
        public readonly float MaxPushingPower, PushPowerDecrease;


        public BallMovementData
        (
            float maxPushingPower,
            float pushPowerDecrease
        )
        {
            MaxPushingPower   = maxPushingPower;
            PushPowerDecrease = pushPowerDecrease;
        }
    }
}