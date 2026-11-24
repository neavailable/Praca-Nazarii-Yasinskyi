namespace MainScene.UI.Timer
{
    public class LevelResult
    {
        public readonly float Time, BestTime, RemainingTime;
        public readonly bool IsNewBest;
        public readonly int TimeBonus;


        public LevelResult(float time, float bestTime, bool isNewBest, float remainingTime, int timeBonus)
        {
            Time          = time;
            BestTime      = bestTime;
            IsNewBest     = isNewBest;
            RemainingTime = remainingTime;
            TimeBonus     = timeBonus;
        }
    }
}
