using UnityEngine;


namespace MainScene.Coin
{
    public class CoinsStartPointMarker : MonoBehaviour
    {
        private void Start() => Validate();

        private void Validate()
        {
            if (!GetComponentInParent<AllCoinsMarker>())
            {
                Debug.LogError("Coin Start Point is not child of All Coins ", this);
            }
        }
    }
}