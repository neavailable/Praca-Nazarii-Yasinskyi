using TMPro;
using R3;
using MainScene.UI.InformationPanel.AbstractMVVM;


namespace MainScene.UI.InformationPanel.Rotation.MVVM
{
    public class RotationView : AbstractView
    {
        public RotationView(TMP_Text text, ReactiveProperty<string> textChanger)
            : base(text, textChanger) { }
    }
}