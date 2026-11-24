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
    // Shows the time left; it turns red near the end, and running out of time loses the level.
    public class LevelTimer : MonoBehaviour
    {
        private const float _warningTime = 30.0f;
        private static readonly Color _warningColor = new (1.0f, 0.35f, 0.3f);

        [SerializeField] private TMP_Text _timeText;

        private LevelTimerLogic _levelTimerLogic;
        private Color _normalColor;
        private IDisposable _finished, _died, _timedOut;


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
            _timedOut = _levelTimerLogic.TimedOut.Subscribe(_ => healthBarModel.Kill());
        }

        private void Awake() => _normalColor = _timeText.color;

        private void Update()
        {
            _levelTimerLogic.Tick(Time.deltaTime);

            float remaining = _levelTimerLogic.Remaining;
            _timeText.text  = TimeFormatter.Format(remaining);
            _timeText.color = remaining < _warningTime ? _warningColor : _normalColor;
        }

        private void OnDestroy()
        {
            _finished?.Dispose();
            _died    ?.Dispose();
            _timedOut?.Dispose();
        }
    }
}
