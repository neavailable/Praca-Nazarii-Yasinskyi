namespace Configuration.Data
{
    public class WindData
    {
        public readonly float MinStrength, MaxStrength;


        public WindData(float minStrength, float maxStrength)
        {
            MinStrength = minStrength;
            MaxStrength = maxStrength;
        }
    }
}
