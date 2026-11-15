namespace Configuration.Data
{
    public class CoinDestroyerData
    {
        public readonly float ShrinkingSpeed, MinScale;
        

        public CoinDestroyerData(float shrinkingSpeed, float minScale)
        {
            ShrinkingSpeed = shrinkingSpeed;
            MinScale       = minScale;
        }
    }
}