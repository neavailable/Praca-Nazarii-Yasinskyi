namespace Configuration.Data
{
    public class BallMenuMovementData
    {
        public float StartPushingPower { get; private set; }


        public BallMenuMovementData(float startPushingPower)
        {
            StartPushingPower = startPushingPower;
        }
    }
}