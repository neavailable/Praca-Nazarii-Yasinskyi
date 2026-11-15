using UnityEngine;


namespace MainScene.RayPoints.GroundPoint
{
    public class GroundPoint : IGetRayPoint
    {
        public Vector3 GetPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(PlayerPointer.Position);

            Plane ground = new Plane(Vector3.up, Vector3.zero);
            return ground.Raycast(ray, out float groundPoint) ?
                ray.GetPoint(groundPoint) : Vector3.zero;
        }   
    }
}
