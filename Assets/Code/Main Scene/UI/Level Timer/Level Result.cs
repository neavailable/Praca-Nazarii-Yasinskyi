namespace MainScene.UI.Timer
{
    public class LevelResult
    {
        public readonly float Time, BestTime;
        public readonly bool IsNewBest;


        public LevelResult(float time, float bestTime, bool isNewBest)
        {
            Time      = time;
            BestTime  = bestTime;
            IsNewBest = isNewBest;
        }
    }
}
