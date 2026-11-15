using UnityEngine;


namespace AllScenes.Statistics
{
    // Per-level results kept between sessions; written in the level, read by the statistics window.
    public class LevelStatistics
    {
        public bool Completed   => PlayerPrefs.GetInt(_completedKey) == 1;
        public float TimePlayed => PlayerPrefs.GetFloat(_timePlayedKey);
        public int MaxCoins     => PlayerPrefs.GetInt(_maxCoinsKey);

        private readonly string _bestTimeKey, _timePlayedKey, _maxCoinsKey, _completedKey;


        public LevelStatistics(string levelName)
        {
            _bestTimeKey   = "Best Time "   + levelName;
            _timePlayedKey = "Time Played " + levelName;
            _maxCoinsKey   = "Max Coins "   + levelName;
            _completedKey  = "Completed "   + levelName;
        }

        public bool TryGetBestTime(out float bestTime)
        {
            bestTime = PlayerPrefs.GetFloat(_bestTimeKey);
            return PlayerPrefs.HasKey(_bestTimeKey);
        }

        public void SaveBestTime(float bestTime) => PlayerPrefs.SetFloat(_bestTimeKey, bestTime);

        public void MarkCompleted() => PlayerPrefs.SetInt(_completedKey, 1);

        public void AddTimePlayed(float seconds) => PlayerPrefs.SetFloat(_timePlayedKey, TimePlayed + seconds);

        public void UpdateMaxCoins(int coins)
        {
            if (coins > MaxCoins) PlayerPrefs.SetInt(_maxCoinsKey, coins);
        }

        public void Save() => PlayerPrefs.Save();
    }
}
