using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using R3;
using MainScene.ClassesParameters;


namespace MainScene.UI.Timer
{
    // Keeps the goal in sight: how many coins are left, then that the finish is open.
    public class ObjectiveText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private readonly List<IDisposable> _subscriptions = new ();
        private int _coinsLeft;


        [Inject]
        private void Constructor(CoinTriggersSubjects coinTriggersSubjects)
        {
            _coinsLeft = coinTriggersSubjects.CoinsTriggersSubjects.Count;

            foreach (Subject<int> coinTrigger in coinTriggersSubjects.CoinsTriggersSubjects)
            {
                _subscriptions.Add(coinTrigger.Subscribe(OnCoin));
            }
            Show();
        }

        private void OnDestroy()
        {
            foreach (IDisposable subscription in _subscriptions) subscription.Dispose();
        }


        // Each coin reports 0 when it appears and its value when it is collected.
        private void OnCoin(int coins)
        {
            if (coins <= 0) return;

            _coinsLeft--;
            Show();
        }

        private void Show()
        {
            _text.text = _coinsLeft > 0 ? "Coins left: " + _coinsLeft : "Finish is open!";
        }
    }
}
