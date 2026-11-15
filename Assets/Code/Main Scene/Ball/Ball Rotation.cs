using UnityEngine;


namespace MainScene.Ball
{
    public class BallRotation : MonoBehaviour
    {
        private BallRotationLogic _ballRotationLogic;


        private void Start()
        {
            BallMovement ballMovement = GetComponentInParent<BallMovement>();

            _ballRotationLogic = new BallRotationLogic
            (
                transform,
                ballMovement.GetComponent<Rigidbody>(),
                ballMovement.BallMovementLogic
            );
        }
        
        private void FixedUpdate() => _ballRotationLogic.RotateX();

        private void OnDestroy() => _ballRotationLogic.Dispose();
    }
}