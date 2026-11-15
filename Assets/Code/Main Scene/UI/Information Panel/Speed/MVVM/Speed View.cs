using TMPro;
using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Speed.MVVM
{
    public class SpeedView : AbstractView
    {
        public SpeedView(TMP_Text text, ReactiveProperty<string> textChanger)
            : base(text, textChanger) { }
    }
}