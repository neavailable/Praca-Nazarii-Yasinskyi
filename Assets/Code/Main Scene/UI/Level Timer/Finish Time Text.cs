using System;
using UnityEngine;
using TMPro;
using Zenject;
using R3;
using AllScenes.Statistics;


namespace MainScene.UI.Timer
{
    // The finish window is shared by win and loss: after a win it shows the time and the time bonus,
    // after running out of time it says so, after losing all health it stays hidden.
    public class FinishTimeText : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _text;

        private IDisposable _result, _timedOut;


        [Inject]
        private void Constructor(LevelTimerLogic levelTimerLogic)
        {
            _result   = levelTimerLogic.Result.Subscribe(ShowResult);
            _timedOut = levelTimerLogic.TimedOut.Subscribe(_ => ShowTimeUp());
        }

        private void OnDestroy()
        {
            _result  ?.Dispose();
            _timedOut?.Dispose();
        }


        private void ShowResult(LevelResult result)
        {
            _panel.SetActive(result != null);
            if (result == null) return;

            string best = result.IsNewBest
                ? "New best time!"
                : "Best: " + TimeFormatter.Format(result.BestTime);

            _text.text = "Time: " + TimeFormatter.Format(result.Time) + "   " + best + "\n"
                       + "Time bonus: +" + result.TimeBonus + " coins";
        }

        private void ShowTimeUp()
        {
            _panel.SetActive(true);
            _text.text = "Time is up!";
        }
    }
}
