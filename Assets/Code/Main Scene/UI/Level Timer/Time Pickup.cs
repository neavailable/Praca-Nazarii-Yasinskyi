using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Configuration.Data;
using MainScene.Ball.BallMarker;
using MainScene.Coin;
using MainScene.Feedback;


namespace MainScene.UI.Timer
{
    // A special coin: the blue clock adds time to the limit, the red one takes it away.
    // It shrinks away like a normal coin.
    [RequireComponent(typeof(Collider))]
    public class TimePickup : MonoBehaviour
    {
        [SerializeField] private bool _isPenalty;

        private LevelTimerLogic _levelTimerLogic;
        private FeedbackPlayer _feedbackPlayer;
        private ShrinkAnimation _shrinkAnimation;
        private float _seconds;


        [Inject]
        private void Constructor
        (
            LevelTimerLogic levelTimerLogic,
            LevelRulesData levelRules,
            FeedbackPlayer feedbackPlayer,
            CoinDestroyerData coinDestroyerData
        )
        {
            _levelTimerLogic = levelTimerLogic;
            _feedbackPlayer  = feedbackPlayer;
            _seconds         = _isPenalty ? -levelRules.TimeTrapPenalty : levelRules.TimeCoinBonus;
            _shrinkAnimation = new ShrinkAnimation
            (
                transform,
                coinDestroyerData.ShrinkingSpeed,
                coinDestroyerData.MinScale
            );
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.TryGetComponent(out BallMarker _)) return;

            _levelTimerLogic.AddTime(_seconds);
            _feedbackPlayer.Play(_isPenalty ? FeedbackType.TimePenalty : FeedbackType.TimeBonus, transform.position);

            GetComponent<Collider>().enabled = false;
            Disappear().Forget();
        }


        private async UniTaskVoid Disappear()
        {
            if (!await _shrinkAnimation.Play()) return;
            gameObject.SetActive(false);
        }
    }
}
