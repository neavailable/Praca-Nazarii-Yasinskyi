using Configuration.Data;


namespace Configuration.Configurations
{
    public class GroundConfiguration : AbstractConfiguration<GroundData>
    {
        public GroundConfiguration() : base("Ground Configuration", "Friction") { }
    }
}