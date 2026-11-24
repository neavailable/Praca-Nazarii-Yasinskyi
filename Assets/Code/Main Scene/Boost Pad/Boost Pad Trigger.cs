using UnityEngine;
using Zenject;
using Configuration.Data;
using MainScene.Ball;
using MainScene.Feedback;


namespace MainScene.BoostPad
{
    // Launches the ball along the pad's arrows, whichever side it came from.
    [RequireComponent(typeof(Collider))]
    public class BoostPadTrigger : MonoBehaviour
    {
        private BoostPadData _boostPadData;
        private FeedbackPlayer _feedbackPlayer;


        [Inject]
        private void Constructor(BoostPadData boostPadData, FeedbackPlayer feedbackPlayer)
        {
            _boostPadData   = boostPadData;
            _feedbackPlayer = feedbackPlayer;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.TryGetComponent(out BallMovement ballMovement)) return;

            Vector3 boostDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            ballMovement.BallMovementLogic.Launch
            (
                boostDirection,
                _boostPadData.BoostPower,
                _boostPadData.MaxSpeed
            );
            _feedbackPlayer.Play(FeedbackType.Boost, ballMovement.transform.position);
        }
    }
}
