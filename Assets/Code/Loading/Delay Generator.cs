using System;


namespace Loading
{
    public class DelayGenerator
    {
        public Action<int> GeneratedDelay;
        
        private readonly int _minDelay, _maxDelay;
        
        
        public DelayGenerator(int minDelay, int maxDelay)
        {
            _minDelay = minDelay;
            _maxDelay = maxDelay;
        }

        public void GenerateDelay()
        {
            GeneratedDelay?.Invoke(UnityEngine.Random.Range(_minDelay, _maxDelay));
        }
    }
}