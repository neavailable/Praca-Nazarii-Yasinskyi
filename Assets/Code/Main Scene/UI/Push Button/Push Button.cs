using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Configuration.Data;
using MainScene.Ball;
using MainScene.Ball.BallMarker;


namespace MainScene.UI.Push
{
    // Works only while the ball is at rest, so it frees a stuck ball but gives no extra speed.
    [RequireComponent(typeof(Button))]
    public class PushButton : MonoBehaviour
    {
        private Button _button;
        private BallPusher _ballPusher;


        [Inject]
        private void Constructor(BallMarker ballMarker, PushData pushData)
        {
            _ballPusher = new BallPusher(ballMarker.GetComponent<BallMovement>(), pushData.PushPower);
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Push);
        }

        private void Update() => _button.interactable = _ballPusher.CanPush;

        private void OnDestroy() => _button.onClick.RemoveListener(Push);


        private void Push() => _ballPusher.Push();
    }
}
