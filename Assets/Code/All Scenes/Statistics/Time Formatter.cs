using UnityEngine;


namespace AllScenes.Statistics
{
    public static class TimeFormatter
    {
        // Whole hundredths avoid "00:60.00" from rounding and keep the format independent of the phone's locale.
        public static string Format(float seconds)
        {
            int hundredths = Mathf.FloorToInt(seconds * 100.0f);

            return string.Format
            (
                "{0:00}:{1:00}.{2:00}",
                hundredths / 6000,
                hundredths / 100 % 60,
                hundredths % 100
            );
        }
    }
}
