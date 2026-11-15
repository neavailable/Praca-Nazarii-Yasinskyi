using UnityEngine;
using MainScene.Ball;


namespace MainMenu.Ball
{
    public class MainMenuBallRotation : MonoBehaviour
    {
        private BallRotationLogic _ballRotationLogic;


        private void Start()
        {
            MainMenuBallMovement ballMovement = GetComponentInParent<MainMenuBallMovement>();

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