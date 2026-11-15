namespace Configuration.Data
{
    public class UICoinData
    {
        public readonly float ChangeSpeed, ScaleFontSpeed, MaxFontSize;
        

        public UICoinData(float changeSpeed, float scaleFontSpeed, float maxFontSize)
        {
            ChangeSpeed    = changeSpeed;
            ScaleFontSpeed = scaleFontSpeed;
            MaxFontSize    = maxFontSize;
        }
    }
}