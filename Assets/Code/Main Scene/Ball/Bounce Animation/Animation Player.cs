using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace MainScene.Ball.BounceAnimation
{
    public class AnimationPlayer
    {
        public async UniTask PlayAnimation
        (
            IBallAnimation animation, 
            float duration, 
            CancellationToken token
        )
        {
            float currentTime = 0.0f;
            while (currentTime < duration)
            {
                token.ThrowIfCancellationRequested();

                animation.PlayAnimation();

                currentTime += Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate();
            }
        }
    }
}