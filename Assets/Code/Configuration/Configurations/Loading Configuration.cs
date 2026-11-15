using Configuration.Data;


namespace Configuration.Configurations
{
    public class LoadingConfiguration : AbstractConfiguration<LoadingData>
    {
        public LoadingConfiguration() : base("Loading Configuration", "LoadingTime") { }
    }
}