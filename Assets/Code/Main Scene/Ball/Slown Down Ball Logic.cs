using System;


namespace MainScene.Ball
{
    public class SlownDownBallLogic
    {
        public Action<float> GetSlowedDirectionAction;
        

        public void GetSlowedDirection(float frictionPower)
        {
            GetSlowedDirectionAction?.Invoke(frictionPower);
        }
    }
}