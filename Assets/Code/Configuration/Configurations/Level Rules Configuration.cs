using Configuration.Data;


namespace Configuration.Configurations
{
    public class LevelRulesConfiguration : AbstractConfiguration<LevelRulesData>
    {
        public LevelRulesConfiguration(string levelName) : base("Level Rules Configuration", levelName) { }
    }
}
