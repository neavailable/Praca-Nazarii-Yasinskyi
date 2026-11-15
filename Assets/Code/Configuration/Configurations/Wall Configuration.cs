using Configuration.Data;


namespace Configuration.Configurations
{
    public class WallConfiguration : AbstractConfiguration<WallData>
    {
        public WallConfiguration() : base("Wall Configuration", "Bounce") { }
    }
}