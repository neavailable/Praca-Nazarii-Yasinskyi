using UnityEngine;


namespace MainScene.Ball
{
    public class PushPowerConverter
    {
        public Vector3 ScreenStartPoint  { private get; set; }
        public Vector3 GroundStartPoint  { private get; set; }
        public Vector3 ScreenFinishPoint { private get; set; }
        public Vector3 GroundFinishPoint { private get; set; }

        private readonly float _maxPushingPower, _pushPowerDecrease;


        public PushPowerConverter(float maxPushingPower, float pushPowerDecrease)
        {
            _maxPushingPower    = maxPushingPower;
            _pushPowerDecrease  = pushPowerDecrease;
        }
        
        public (float distance, Vector3 direction) ConvertToPushPower()
        {
            float distance = Vector3.Distance(ScreenStartPoint, ScreenFinishPoint) / _pushPowerDecrease;
            Vector3 normalizedDirection = -(GroundFinishPoint - GroundStartPoint).normalized;

            if (distance > _maxPushingPower) distance = _maxPushingPower;
            
            return (distance, normalizedDirection);
        }
    }
}