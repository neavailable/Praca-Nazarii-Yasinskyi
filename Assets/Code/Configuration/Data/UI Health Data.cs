namespace Configuration.Data
{
    public class UIHealthData
    {
        public readonly float Delay;
        public readonly int DefaultHealth, MaxFontSize;


        public UIHealthData(float delay, int defaultHealth, int maxFontSize)
        {
            Delay = delay;
            DefaultHealth  = defaultHealth;
            MaxFontSize    = maxFontSize;
        }
    }
}