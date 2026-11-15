using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


namespace Loading
{
    public class LoadingBar : IDisposable
    {
        public Action LoadingBarFilled;
        
        private readonly DelayGenerator _delayGenerator;
        private readonly Slider _slider;
        

        public LoadingBar(DelayGenerator delayGenerator, Slider slider)
        {
            _delayGenerator = delayGenerator;
            _slider         = slider;
            
            _delayGenerator.GeneratedDelay += StartFillingBar;
        }

        public void Dispose()
        {
            _delayGenerator.GeneratedDelay -= StartFillingBar;
        }
        

        private void StartFillingBar(int delay) => Fill(delay).Forget();
        
        private async UniTaskVoid Fill(int delay)
        {
            float currentTime = 0.0f;
            while (currentTime < delay)
            {
                _slider.value = currentTime / delay;
                
                await UniTask.WaitForFixedUpdate();
                currentTime += Time.fixedDeltaTime;
            }
            
            LoadingBarFilled?.Invoke();
        }
    }
}