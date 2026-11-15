using Configuration.Data;


namespace Configuration.Configurations
{
    public class UICoinConfiguration : AbstractConfiguration<UICoinData>
    {
        public UICoinConfiguration() : base("UI Configuration", "Coin") { }
    }
}