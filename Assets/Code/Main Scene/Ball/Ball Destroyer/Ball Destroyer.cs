using System;
using UnityEngine;
using Zenject;
using R3;
using MainScene.Finish.FinishTrigger;
using MainScene.UI.HealthBar.MVVM;


namespace MainScene.Ball.BallDestroyer
{
    [RequireComponent(typeof(HealthBarModel))]
    public class BallDestroyer : MonoBehaviour
    {
        public Subject<Unit> BallDestroyed;
        
        private FinishTrigger _finishTrigger;
        private IDisposable _win, _die;
        

        [Inject]
        private void Constructor(FinishTrigger finishTrigger)
        {
            _finishTrigger = finishTrigger;
        }
        
        private void Awake() => BallDestroyed = new ();
        
        private void Start()
        {
            _die = GetComponent<HealthBarModel>().Die.Subscribe(DestroyBall);
            _win = _finishTrigger.BallCollidedFinished.Subscribe(DestroyBall);
        }

        private void DestroyBall(Unit _)
        {
            BallDestroyed?.OnNext(_);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _win.Dispose();
            _die.Dispose();
        }
    }
}