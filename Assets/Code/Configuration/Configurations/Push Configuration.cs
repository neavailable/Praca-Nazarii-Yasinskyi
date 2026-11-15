using Configuration.Data;


namespace Configuration.Configurations
{
    public class PushConfiguration : AbstractConfiguration<PushData>
    {
        public PushConfiguration() : base("Push Configuration", "Push") { }
    }
}
