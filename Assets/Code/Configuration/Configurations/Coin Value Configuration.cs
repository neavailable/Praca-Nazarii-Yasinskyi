using Configuration.Data;


namespace Configuration.Configurations
{
    public class CoinValueConfiguration : AbstractConfiguration<CoinValueData>
    {
        public CoinValueConfiguration() : base("Coin Configuration", "Value") { }
    }
}