using System;
using UnityEngine;
using Zenject;
using MainScene.Ball.BallMarker;
using MainScene.Feedback;


namespace MainScene.Enemy
{
    public class EnemyDeathCollider : MonoBehaviour
    {
        public Action BallPushed;

        private FeedbackPlayer _feedbackPlayer;
        private bool _defeated;


        [Inject]
        private void Constructor(FeedbackPlayer feedbackPlayer) => _feedbackPlayer = feedbackPlayer;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BallMarker _))
            {
                if (!_defeated) _feedbackPlayer.Play(FeedbackType.EnemyDefeated, transform.position);
                _defeated = true;

                BallPushed?.Invoke();
            }
        }
    }
}
