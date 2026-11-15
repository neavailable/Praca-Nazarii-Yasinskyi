using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;


namespace AdSettings
{
    public class RewardedAd : MonoBehaviour, IRewardedVideoAdListener
    {
        private const string _appKey = "f8138a3095f2e67e93cd11aa398a51665a4a59926f9d2fbe";


        private void Start()
        {
            Appodeal.initialize(_appKey, Appodeal.REWARDED_VIDEO);
            
            Appodeal.setRewardedVideoCallbacks(this);
        }

        public void ShowAd()
        {
            if (Appodeal.canShow(Appodeal.REWARDED_VIDEO) && !Appodeal.isPrecache(Appodeal.REWARDED_VIDEO))
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO);
            }
        }

        public void onRewardedVideoLoaded(bool precache) { }
        
        public void onRewardedVideoFailedToLoad() { }
        
        public void onRewardedVideoShowFailed() { }
        
        public void onRewardedVideoShown() { }
        
        public void onRewardedVideoFinished(double amount, string name) { }
        
        public void onRewardedVideoClosed(bool finished) { }
        
        public void onRewardedVideoExpired() { }
        
        public void onRewardedVideoClicked() { }
    }
}