using UnityEngine;


namespace MainScene.Wall.WallsMarker
{
    public class WallMarker : MonoBehaviour
    {
        private void Start() => Validate();

        private void Validate()
        {
            if (!GetComponentInParent<AllWallsMarker>())
            {
                Debug.LogError("Wall is not child of All Walls ", this);
            }
        }
    }
}