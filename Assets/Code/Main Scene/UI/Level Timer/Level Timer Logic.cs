using UnityEngine;
using R3;
using AllScenes.Statistics;
using Configuration.Data;


namespace MainScene.UI.Timer
{
    // Counts the level time up (for the best time) and the time limit down (for losing).
    // Every whole second left at the finish is paid out as coins, multiplied like the level's coins.
    public class LevelTimerLogic
    {
        public readonly ReactiveProperty<LevelResult> Result = new ();
        public readonly Subject<Unit> TimedOut = new ();

        public float Elapsed { get; private set; }
        public bool IsRunning { get; private set; } = true;
        public float Remaining => Mathf.Max(_levelRules.TimeLimit + _extraTime - Elapsed, 0.0f);

        private readonly LevelStatistics _levelStatistics;
        private readonly LevelRulesData _levelRules;
        private float _extraTime;


        public LevelTimerLogic(LevelStatistics levelStatistics, LevelRulesData levelRules)
        {
            _levelStatistics = levelStatistics;
            _levelRules      = levelRules;
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            Elapsed += deltaTime;
            if (Remaining > 0.0f) return;

            Stop();
            TimedOut.OnNext(Unit.Default);
        }

        public void AddTime(float seconds)
        {
            if (IsRunning) _extraTime += seconds;
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

            int timeBonus = Mathf.FloorToInt(Remaining) * _levelRules.CoinMultiplier;
            Result.Value = new LevelResult(Elapsed, bestTime, isNewBest, Remaining, timeBonus);
        }
    }
}
