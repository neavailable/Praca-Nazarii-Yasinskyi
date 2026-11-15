using UnityEngine;


namespace MainScene.Enemy
{
    [RequireComponent(typeof(CactusMovement))]
    public class CactusRotation : MonoBehaviour
    {
        private CactusRotationLogic _cactusRotationLogic;


        private void OnEnable()
        {
            _cactusRotationLogic = new 
            (
                transform,
                GetComponent<CactusMovement>().CactusMovementLogic
            );
        }

        private void OnDisable() => _cactusRotationLogic.Dispose();
    }
}