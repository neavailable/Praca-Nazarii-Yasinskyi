using Configuration.Data;
 
 
namespace Configuration.Configurations
{
    public class CoinMovementConfiguration : AbstractConfiguration<CoinMovementData>
    {
        public CoinMovementConfiguration() : base("Coin Configuration", "Movement") { }
    }
}