using Configuration.Data;


namespace Configuration.Configurations
{
    public class WindConfiguration : AbstractConfiguration<WindData>
    {
        public WindConfiguration() : base("Wind Configuration", "Wind") { }
    }
}
