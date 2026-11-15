using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using R3;
using AllScenes.Statistics;
using MainScene.ClassesParameters;


namespace MainScene.UI.Timer
{
    // Adds the time spent in the level and keeps the most coins collected in one run.
    // Time is saved when the level is left (retry, menu) and when the app goes to the background.
    public class LevelStatisticsRecorder : MonoBehaviour
    {
        private readonly List<IDisposable> _coinsCollected = new ();

        private LevelStatistics _levelStatistics;
        private LevelTimerLogic _levelTimerLogic;
        private float _savedTime;
        private int _coins;


        [Inject]
        private void Constructor
        (
            LevelStatistics levelStatistics,
            LevelTimerLogic levelTimerLogic,
            CoinTriggersSubjects coinTriggersSubjects
        )
        {
            _levelStatistics = levelStatistics;
            _levelTimerLogic = levelTimerLogic;

            foreach (Subject<int> coinTrigger in coinTriggersSubjects.CoinsTriggersSubjects)
            {
                _coinsCollected.Add(coinTrigger.Subscribe(AddCoins));
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) SaveTimePlayed();
        }

        private void OnDestroy()
        {
            SaveTimePlayed();
            foreach (IDisposable coinCollected in _coinsCollected) coinCollected.Dispose();
        }


        private void AddCoins(int coins)
        {
            _coins += coins;
            _levelStatistics.UpdateMaxCoins(_coins);
        }

        private void SaveTimePlayed()
        {
            _levelStatistics.AddTimePlayed(_levelTimerLogic.Elapsed - _savedTime);
            _levelStatistics.Save();

            _savedTime = _levelTimerLogic.Elapsed;
        }
    }
}
