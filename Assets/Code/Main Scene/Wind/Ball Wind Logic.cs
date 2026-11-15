using MainScene.Ball;


namespace MainScene.Wind
{
    public class BallWindLogic
    {
        private readonly IBallMovementContainer _ballMovement;
        private readonly LevelWind _levelWind;


        public BallWindLogic(IBallMovementContainer ballMovement, LevelWind levelWind)
        {
            _ballMovement = ballMovement;
            _levelWind    = levelWind;
        }

        // A gentle constant drift, so the wind moves a ball at rest too.
        public void Blow()
        {
            AbstractBallMovementLogic movementLogic = _ballMovement.BallMovementLogic;
            if (movementLogic == null) return;

            movementLogic.Drift = _levelWind.Velocity;
        }
    }
}
