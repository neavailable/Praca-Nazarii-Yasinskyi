using Zenject;
using Configuration.Data;
using MainScene.UserInput;
using MainScene.Finish.FinishTrigger;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToBall : Binder
    {
        private readonly BallInput _ballInput;
        private readonly FinishTrigger _finishTrigger;
        private readonly BallMovementData _ballMovementData;
        private readonly BallBounceAnimationData _ballBounceAnimation;
        private readonly UIHealthData _uiHealthData;
        
        
        public BindToBall
        (
            DiContainer container, 
            BallInput ballInput,
            FinishTrigger finishTrigger,
            BallMovementData ballMovementData,
            BallBounceAnimationData ballBounceAnimationData,
            UIHealthData uiHealthData
        ) : base(container)
        {
            _ballInput           = ballInput;
            _finishTrigger       = finishTrigger;
            _ballMovementData    = ballMovementData;
            _ballBounceAnimation = ballBounceAnimationData;
            _uiHealthData        = uiHealthData;
        }

        public override void Bind()
        {
            BindBallInput();
            BindFinishTrigger();
            BindBallMovementData();
            BindBallBounceAnimationData();
            BindUIHealthData();
        }


        private void BindBallInput()
        {
            Container.Bind<BallInput>()
                .FromInstance(_ballInput).AsSingle();
        }

        private void BindFinishTrigger()
        {
            Container.Bind<FinishTrigger>()
                .FromInstance(_finishTrigger).AsSingle();
        }
        
        private void BindBallMovementData()
        {
            Container.Bind<BallMovementData>()
                .FromInstance(_ballMovementData).AsSingle();
        }

        private void BindBallBounceAnimationData()
        {
            Container.Bind<BallBounceAnimationData>()
                .FromInstance(_ballBounceAnimation).AsSingle();
        }

        private void BindUIHealthData()
        {
            Container.Bind<UIHealthData>()
                .FromInstance(_uiHealthData);
        }
    }
}