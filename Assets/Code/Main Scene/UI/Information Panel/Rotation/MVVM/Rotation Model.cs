using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Rotation.MVVM
{
    public class RotationModel : AbstractModel
    {
        public RotationModel(ReactiveProperty<float> meshRotation) : base(meshRotation) { }
    }
}