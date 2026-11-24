using Zenject;
using MainScene.Feedback;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindToFeedback : Binder
    {
        public BindToFeedback(DiContainer container) : base(container) { }

        public override void Bind() => BindFeedbackPlayer();


        // The player lives in the level's HUD.
        private void BindFeedbackPlayer()
        {
            Container.Bind<FeedbackPlayer>().FromComponentInHierarchy().AsSingle();
        }
    }
}
