using System;
using UnityEngine;
using TMPro;
using Zenject;
using R3;
using AllScenes.Statistics;
using MainScene.Finish.FinishTrigger;
using MainScene.UI.HealthBar.MVVM;


namespace MainScene.UI.Timer
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;

        private LevelTimerLogic _levelTimerLogic;
        private IDisposable _finished, _died;


        // Subscribing at injection, before the ball destroyer does in Start,
        // makes the result ready by the time the finish window opens.
        [Inject]
        private void Constructor
        (
            LevelTimerLogic levelTimerLogic,
            FinishTrigger finishTrigger,
            HealthBarModel healthBarModel
        )
        {
            _levelTimerLogic = levelTimerLogic;

            _finished = finishTrigger.BallCollidedFinished.Subscribe(_ => _levelTimerLogic.Finish());
            _died     = healthBarModel.Die.Subscribe(_ => _levelTimerLogic.Stop());
        }

        private void Update()
        {
            _levelTimerLogic.Tick(Time.deltaTime);
            _timeText.text = TimeFormatter.Format(_levelTimerLogic.Elapsed);
        }

        private void OnDestroy()
        {
            _finished?.Dispose();
            _died    ?.Dispose();
        }
    }
}
