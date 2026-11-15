using UnityEngine;


namespace MainScene.RayPoints.ScreenPoint
{
    public class ScreenPoint : IGetRayPoint
    {
        public Vector3 GetPoint()
        {
            return PlayerPointer.Position;
        }
    }
}
