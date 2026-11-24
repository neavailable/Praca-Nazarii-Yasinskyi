using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using R3;
using MainScene.Ball.BallMarker;
using MainScene.ClassesParameters;
using MainScene.Feedback;
using MainScene.Finish.FinishTrigger;
using MainScene.UI.HealthBar.MVVM;


namespace MainScene.UI.Timer
{
    // Answers the ball's own events with sounds and particles: coins, healing, damage, winning and losing.
    public class LevelFeedback : MonoBehaviour
    {
        private readonly List<IDisposable> _subscriptions = new ();

        private FeedbackPlayer _feedbackPlayer;
        private HealthBarModel _healthBarModel;
        private Transform _ball;
        private int _health;


        [Inject]
        private void Constructor
        (
            FeedbackPlayer feedbackPlayer,
            HealthBarModel healthBarModel,
            BallMarker ballMarker,
            FinishTrigger finishTrigger,
            CoinTriggersSubjects coinTriggersSubjects
        )
        {
            _feedbackPlayer = feedbackPlayer;
            _healthBarModel = healthBarModel;
            _ball           = ballMarker.transform;
            _health         = healthBarModel.DefaultHealth;

            foreach (Subject<int> coinTrigger in coinTriggersSubjects.CoinsTriggersSubjects)
            {
                _subscriptions.Add(coinTrigger.Subscribe(OnCoin));
            }
            _subscriptions.Add(finishTrigger.BallCollidedFinished.Subscribe(_ => Play(FeedbackType.Win)));
            _subscriptions.Add(healthBarModel.Die.Subscribe(_ => Play(FeedbackType.Lose)));

            _healthBarModel.HealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            foreach (IDisposable subscription in _subscriptions) subscription.Dispose();
            _healthBarModel.HealthChanged -= OnHealthChanged;
        }


        private void OnCoin(int coins)
        {
            if (coins > 0) Play(FeedbackType.Coin);
        }

        private void OnHealthChanged(int health)
        {
            if (health > 0) Play(health > _health ? FeedbackType.Heal : FeedbackType.Damage);
            _health = health;
        }

        private void Play(FeedbackType type) => _feedbackPlayer.Play(type, _ball.position);
    }
}
