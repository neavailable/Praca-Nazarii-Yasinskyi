using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MainScene.Wall.WallsMarker;


namespace MainScene.Ball.BounceAnimation
{
    public class BounceAnimationLogic
    {
        private float HalfAnimationDuration => _animationDuration / 2.0f;
        private readonly AnimationPlayer _animationPlayer;
        private readonly IBallAnimation _ballSqueezingAnimation, _ballExtendingAnimation;
        private readonly IBallAnimation _ballTurningRedAnimation;
        private readonly float _animationDuration;

        
        public BounceAnimationLogic
        (
            BallMovement ballMovement, 
            Transform transform,
            MeshRenderer meshRenderer,
            Color targetColor,
            float animationDuration,
            float squeezingExtendingSpeedDivider,
            float colorChangeSpeed
        )
        {
            _animationDuration = animationDuration;

            _animationPlayer         = new AnimationPlayer();
            _ballSqueezingAnimation  = new BallSqueezingAnimation
            (
                ballMovement,
                transform,
                squeezingExtendingSpeedDivider
            );
            _ballExtendingAnimation  = new BallExtendingAnimation
            (
                ballMovement,
                transform,
                squeezingExtendingSpeedDivider
            );
            _ballTurningRedAnimation = new BallTurningRedAnimation
            (
                meshRenderer,
                targetColor,
                colorChangeSpeed
            );
        }

        public void StartBouncing(GameObject gameObject)
        {
            PlayBounceAnimation(gameObject).Forget();
        }


        private bool IsWall(GameObject collidedObject)
        {
            return collidedObject.TryGetComponent(out WallMarker _);
        }

        private async UniTaskVoid PlayBounceAnimation(GameObject gameObject)
        {
            CancellationTokenSource cancellationTokenSource = new (); 
            
            UniTask turningRedTask = _animationPlayer.PlayAnimation
            (
                _ballTurningRedAnimation,
                _animationDuration,
                cancellationTokenSource.Token
            );
            
            bool collidedObjectIsWall = IsWall(gameObject);
            if (collidedObjectIsWall)
            {
                await _animationPlayer.PlayAnimation
                (
                    _ballSqueezingAnimation,
                    HalfAnimationDuration,
                    cancellationTokenSource.Token
                );
                await _animationPlayer.PlayAnimation
                (
                    _ballExtendingAnimation,
                    HalfAnimationDuration,
                    cancellationTokenSource.Token
                );
            }

            await turningRedTask;

            if (collidedObjectIsWall)
            {
                _ballSqueezingAnimation.FinishBouncingAnimation();
                _ballExtendingAnimation.FinishBouncingAnimation();
            }
            _ballTurningRedAnimation.FinishBouncingAnimation();
            
            cancellationTokenSource.Dispose();
        }
    }
}