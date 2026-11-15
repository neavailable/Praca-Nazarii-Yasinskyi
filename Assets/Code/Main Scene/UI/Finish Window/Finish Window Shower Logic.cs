using System;
using UnityEngine;
using R3;
using MainScene.Ball.BallDestroyer;


namespace MainScene.UI.FinishWindow
{
    public class FinishWindowShowerLogic : IDisposable
    {
        private readonly GameObject _looseUI;
        private readonly IDisposable _ballDestroyed;

        
        public FinishWindowShowerLogic
        (
            BallDestroyer ballDestroyer,
            GameObject looseUI
        )
        {
            _looseUI       = looseUI;

            _ballDestroyed = ballDestroyer.BallDestroyed.
                Subscribe(ShowLooseUI);
        }
        
        public void Dispose() => _ballDestroyed.Dispose();
            

        private void ShowLooseUI(Unit _) => _looseUI.SetActive(true);
    }
}