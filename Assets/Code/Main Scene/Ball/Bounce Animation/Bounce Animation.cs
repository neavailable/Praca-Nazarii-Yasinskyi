using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Ball.BounceAnimation
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BounceAnimation : MonoBehaviour
    {
        private BounceBall _bounceBall;
        private BounceAnimationLogic _bounceAnimationLogic;
        private BallBounceAnimationData _ballBounceAnimationData;


        [Inject]
        private void Constructor(BallBounceAnimationData ballBounceAnimationData)
        {
            _ballBounceAnimationData = ballBounceAnimationData;
        }
        
        private void OnEnable()
        {
            _bounceBall = GetComponentInParent<BounceBall>();
            
            _bounceAnimationLogic = new BounceAnimationLogic
            (
                GetComponentInParent<BallMovement>(),
                transform,
                GetComponent<MeshRenderer>(),
                _ballBounceAnimationData.TargetColor,
                _ballBounceAnimationData.AnimationDuration,
                _ballBounceAnimationData.SqueezingExtendingSpeedDivider,
                _ballBounceAnimationData.ColorChangeSpeed
            );

            _bounceBall.BounceBallLogic.BallBounced += _bounceAnimationLogic.StartBouncing;
        }
        
        private void OnDisable()
        {
            _bounceBall.BounceBallLogic.BallBounced -= _bounceAnimationLogic.StartBouncing;
        }
    }
}