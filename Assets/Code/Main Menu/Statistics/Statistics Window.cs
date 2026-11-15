using UnityEngine;
using TMPro;
using AllScenes.Statistics;


namespace MainMenu.Statistics
{
    // One row per level, refreshed every time the window opens.
    public class StatisticsWindow : MonoBehaviour
    {
        [SerializeField] private string[] _levelNames;
        [SerializeField] private TMP_Text[] _completedTexts, _timePlayedTexts, _bestTimeTexts, _maxCoinsTexts;


        private void OnEnable()
        {
            for (int i = 0; i < _levelNames.Length; i++)
            {
                LevelStatistics levelStatistics = new (_levelNames[i]);

                _completedTexts[i] .text = levelStatistics.Completed ? "Yes" : "No";
                _timePlayedTexts[i].text = TimeFormatter.Format(levelStatistics.TimePlayed);
                _bestTimeTexts[i]  .text = levelStatistics.TryGetBestTime(out float bestTime)
                    ? TimeFormatter.Format(bestTime)
                    : "-";
                _maxCoinsTexts[i]  .text = levelStatistics.MaxCoins.ToString();
            }
        }
    }
}
