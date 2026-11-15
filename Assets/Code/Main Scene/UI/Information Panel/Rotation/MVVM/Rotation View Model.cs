using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Rotation.MVVM
{
    public class RotationViewModel : AbstractViewModel
    {
        public RotationViewModel(ReactiveProperty<float> data) : base(data, "Angle:") { }
    }
}