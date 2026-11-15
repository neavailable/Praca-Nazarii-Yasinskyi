using UnityEngine;


namespace Configuration.Data
{
    public class BallBounceAnimationData
    {
        public readonly Color TargetColor;
        public readonly float AnimationDuration;
        public readonly float SqueezingExtendingSpeedDivider, ColorChangeSpeed;
        

        public BallBounceAnimationData
        (
            float animationDuration,
            float squeezingExtendingSpeedDivider,
            float colorChangeSpeed,
            int targetColorR,
            int targetColorG,
            int targetColorB,
            int targetColorA
        )
        {
            AnimationDuration              = animationDuration;
            SqueezingExtendingSpeedDivider = squeezingExtendingSpeedDivider;
            ColorChangeSpeed               = colorChangeSpeed;
            
            TargetColor = new Color32
            (
                (byte)targetColorR,
                (byte)targetColorG,
                (byte)targetColorB,
                (byte)targetColorA
            );
        }
    }
}