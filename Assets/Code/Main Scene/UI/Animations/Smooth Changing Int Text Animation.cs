using Cysharp.Threading.Tasks;
using R3;


namespace MainScene.UI.Animations
{
    public class SmoothChangingIntTextAnimation
    {
        public readonly Subject<Unit> PulsateText;
        public readonly Subject<Unit> FinishedTextPulsation;
        
        private readonly ReactiveProperty<string> _coinsText;
        private readonly float _changeSpeed;


        public SmoothChangingIntTextAnimation
        (
            ReactiveProperty<string> coinsText, 
            float changeSpeed
        )
        {
            _coinsText   = coinsText;
            _changeSpeed = changeSpeed;
            
            PulsateText           = new ();
            FinishedTextPulsation = new ();
        }
        
        public async UniTaskVoid Change(int target)
        {
            int current = int.Parse(_coinsText.Value);

            if (current >= target) return;

            int k = current < target ? 1 : -1;
            
            while (current != target)
            {
                current += k;
                _coinsText.Value = current.ToString();
                PulsateText.OnNext(Unit.Default);
                
                await UniTask.WaitForSeconds(_changeSpeed);
            }
            FinishedTextPulsation.OnNext(Unit.Default);
        }
    }
}