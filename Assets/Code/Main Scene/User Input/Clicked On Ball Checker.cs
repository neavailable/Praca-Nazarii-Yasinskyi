using MainScene.Ball.BallMarker;
using UnityEngine;


namespace MainScene.UserInput
{
    public class ClickedOnBallChecker
    {
        // Triggers (boost pads, pickups, the finish) must not hide a ball standing inside them.
        public bool CheckClickedOnBall(Ray ray)
        {
            if
            (
                Physics.Raycast
                (
                    ray,
                    out RaycastHit hit,
                    Mathf.Infinity,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore
                ) &&
                hit.collider.TryGetComponent(out BallMarker _)
            )
            {
                return true;
            }
            return false;
        }
    }
}