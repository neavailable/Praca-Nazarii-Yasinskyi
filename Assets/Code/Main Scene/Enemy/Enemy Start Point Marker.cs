using UnityEngine;


namespace MainScene.Enemy
{
    public class EnemyStartPointMarker : MonoBehaviour
    {
        private void Start() => Validate();
        
        private void Validate()
        {
            if (!GetComponentInParent<AllEnemiesMarker>())
            {
                Debug.LogError("Enemy Start Point is not child of All Enemies ", this);
            }
        }
    }
}