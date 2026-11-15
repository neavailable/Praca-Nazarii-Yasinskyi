using UnityEngine;


namespace MainScene.Ball.BounceAnimation
{
    public class BallTurningRedAnimation : IBallAnimation
    {
        private readonly MeshRenderer _meshRenderer;
        private readonly Color _defaultColor, _targetColor;
        private readonly float _colorChangeSpeed;


        public BallTurningRedAnimation
        (
            MeshRenderer meshRenderer,
            Color targetColor,
            float colorChangeSpeed
        )
        {
            _meshRenderer              = meshRenderer;
            _targetColor               = targetColor;
            _colorChangeSpeed          = colorChangeSpeed;
            _defaultColor              = meshRenderer.material.color;
        }
        
        public void PlayAnimation()
        {
            _meshRenderer.material.color = Color.Lerp
            (
                _meshRenderer.material.color,
                _targetColor,
                _colorChangeSpeed * Time.fixedDeltaTime
            );
        }
        
        public void FinishBouncingAnimation() => _meshRenderer.material.color = _defaultColor;
    }
}