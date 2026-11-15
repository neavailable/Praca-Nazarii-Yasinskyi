using Configuration.Data;


namespace Configuration.Configurations
{
    public class HealthConfiguration : AbstractConfiguration<UIHealthData>
    {
        public HealthConfiguration() : base("UI Configuration", "Health") { }        
    }
}