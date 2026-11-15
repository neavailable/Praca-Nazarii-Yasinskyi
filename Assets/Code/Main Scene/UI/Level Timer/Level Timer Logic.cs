using R3;
using AllScenes.Statistics;


namespace MainScene.UI.Timer
{
    public class LevelTimerLogic
    {
        public readonly ReactiveProperty<LevelResult> Result = new ();

        public float Elapsed { get; private set; }
        public bool IsRunning { get; private set; } = true;

        private readonly LevelStatistics _levelStatistics;


        public LevelTimerLogic(LevelStatistics levelStatistics) => _levelStatistics = levelStatistics;

        public void Tick(float deltaTime)
        {
            if (IsRunning) Elapsed += deltaTime;
        }

        public void Stop() => IsRunning = false;

        public void Finish()
        {
            if (!IsRunning) return;
            Stop();

            bool isNewBest = !_levelStatistics.TryGetBestTime(out float bestTime) || Elapsed < bestTime;
            if (isNewBest)
            {
                bestTime = Elapsed;
                _levelStatistics.SaveBestTime(bestTime);
            }
            _levelStatistics.MarkCompleted();
            _levelStatistics.Save();

            Result.Value = new LevelResult(Elapsed, bestTime, isNewBest);
        }
    }
}
