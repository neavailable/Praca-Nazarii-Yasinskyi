using Configuration.Data;


namespace Configuration.Configurations
{
    public class CoinRotationConfiguration : AbstractConfiguration<CoinRotationData>
    {
        public CoinRotationConfiguration() : base("Coin Configuration", "Rotation") { }
    }
}