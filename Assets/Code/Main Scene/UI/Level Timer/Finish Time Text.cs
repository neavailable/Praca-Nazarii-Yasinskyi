using System;
using UnityEngine;
using TMPro;
using Zenject;
using R3;
using AllScenes.Statistics;


namespace MainScene.UI.Timer
{
    // The finish window is shared by win and loss, the time is shown only after a win.
    public class FinishTimeText : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _text;

        private IDisposable _result;


        [Inject]
        private void Constructor(LevelTimerLogic levelTimerLogic)
        {
            _result = levelTimerLogic.Result.Subscribe(ShowResult);
        }

        private void OnDestroy() => _result?.Dispose();


        private void ShowResult(LevelResult result)
        {
            _panel.SetActive(result != null);
            if (result == null) return;

            string best = result.IsNewBest
                ? "New best time!"
                : "Best: " + TimeFormatter.Format(result.BestTime);

            _text.text = "Time: " + TimeFormatter.Format(result.Time) + "\n" + best;
        }
    }
}
