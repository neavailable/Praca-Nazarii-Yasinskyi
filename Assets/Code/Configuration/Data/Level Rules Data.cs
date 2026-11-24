namespace Configuration.Data
{
    // Difficulty of one level: how much time it gives, how it rewards and what the intro says.
    public class LevelRulesData
    {
        public readonly string Title, Intro;
        public readonly float TimeLimit, TimeCoinBonus, TimeTrapPenalty;
        public readonly int CoinMultiplier;


        public LevelRulesData
        (
            string title,
            string intro,
            float timeLimit,
            float timeCoinBonus,
            float timeTrapPenalty,
            int coinMultiplier
        )
        {
            Title           = title;
            Intro           = intro;
            TimeLimit       = timeLimit;
            TimeCoinBonus   = timeCoinBonus;
            TimeTrapPenalty = timeTrapPenalty;
            CoinMultiplier  = coinMultiplier;
        }
    }
}
