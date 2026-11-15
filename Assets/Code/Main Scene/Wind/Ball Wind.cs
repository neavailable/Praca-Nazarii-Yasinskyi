using UnityEngine;
using Zenject;
using MainScene.Ball;


namespace MainScene.Wind
{
    [RequireComponent(typeof(BallMovement))]
    public class BallWind : MonoBehaviour
    {
        private BallWindLogic _ballWindLogic;
        private LevelWind _levelWind;


        [Inject]
        private void Constructor(LevelWind levelWind) => _levelWind = levelWind;

        private void Start()
        {
            _ballWindLogic = new BallWindLogic(GetComponent<BallMovement>(), _levelWind);
        }

        private void FixedUpdate() => _ballWindLogic.Blow();
    }
}
