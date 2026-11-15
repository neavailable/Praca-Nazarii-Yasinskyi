using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Speed.MVVM
{
    public class SpeedModel : AbstractModel
    {
        public SpeedModel(ReactiveProperty<float> meshRotation) : base(meshRotation) { }
    }
}