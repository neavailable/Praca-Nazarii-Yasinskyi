namespace Configuration.Data
{
    public class LoadingData
    {
        public readonly int MinLoadingTime, MaxLoadingTime;


        public LoadingData(int minLoadingTime, int maxLoadingTime)
        {
            MinLoadingTime = minLoadingTime;
            MaxLoadingTime = maxLoadingTime;
        }
    }
}