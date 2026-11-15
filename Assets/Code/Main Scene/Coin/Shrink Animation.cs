using UnityEngine;
using Cysharp.Threading.Tasks;


namespace MainScene.Coin
{
    // The collect animation of coins, also used by heal pickups.
    public class ShrinkAnimation
    {
        private readonly Transform _transform;
        private readonly float _shrinkingSpeed, _minScale;


        public ShrinkAnimation(Transform transform, float shrinkingSpeed, float minScale)
        {
            _transform      = transform;
            _shrinkingSpeed = shrinkingSpeed;
            _minScale       = minScale;
        }

        public async UniTask Play()
        {
            while (ContinueShrinking())
            {
                _transform.localScale = new Vector3
                (
                    _transform.localScale.x - _shrinkingSpeed * Time.fixedDeltaTime,
                    _transform.localScale.y - _shrinkingSpeed * Time.fixedDeltaTime,
                    _transform.localScale.z
                );

                await UniTask.WaitForFixedUpdate();
            }
        }


        private bool ContinueShrinking()
        {
            return _transform.localScale.x > _minScale || _transform.localScale.y > _minScale;
        }
    }
}
