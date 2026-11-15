using Configuration.Data;


namespace Configuration.Configurations
{
    public class BallMovementConfiguration : AbstractConfiguration<BallMovementData>
    {
        public BallMovementConfiguration() : base("Ball Configuration", "Movement") { }
    }
}