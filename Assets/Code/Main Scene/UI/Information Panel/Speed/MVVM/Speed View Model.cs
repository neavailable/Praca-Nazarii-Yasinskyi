using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Speed.MVVM
{
    public class SpeedViewModel : AbstractViewModel
    {
        public SpeedViewModel(ReactiveProperty<float> data) : base(data, "Speed:") { }
    }
}