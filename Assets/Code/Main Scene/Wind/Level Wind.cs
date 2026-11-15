using UnityEngine;
using Configuration.Data;


namespace MainScene.Wind
{
    public class LevelWind
    {
        public readonly Vector3 Direction;
        public readonly float Strength;

        public Vector3 Velocity => Direction * Strength;


        public LevelWind(WindData windData)
        {
            float angle = Random.Range(0.0f, 360.0f);

            Direction = Quaternion.Euler(0.0f, angle, 0.0f) * Vector3.forward;
            Strength  = Random.Range(windData.MinStrength, windData.MaxStrength);
        }
    }
}
