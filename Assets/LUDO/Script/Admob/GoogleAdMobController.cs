using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class GoogleAdMobController : MonoBehaviour
{
    //AdMob ad ids
#if UNITY_ANDROID || UNITY_EDITOR|| UNITY_WEBGL|| UNITY_STANDALONE_WIN

    public string ADMOB_BANNER_ID;
    public string ADMOB_INTERSTITITIAL_ID;

#elif UNITY_IPHONE || UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE_WIN

    public string ADMOB_BANNER_ID;
    public string ADMOB_INTERSTITITIAL_ID;

#endif

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

#region UNITY MONOBEHAVIOR METHODS

    public static GoogleAdMobController Instance;

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            RequestBannerAd();
            RequestAndLoadInterstitialAd();
        });
    }

#endregion

#region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

#endregion

#region BANNER ADS

    public void RequestBannerAd()
    {        
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(ADMOB_BANNER_ID, AdSize.Banner, AdPosition.Top);

        // Add Event Handlers
        bannerView.OnAdLoaded += OnBannerLoaded;

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    private void OnBannerLoaded(System.Object sender, EventArgs args)
    {
        bannerView.Show();
    }

#endregion

#region INTERSTITIAL ADS

    private void RequestAndLoadInterstitialAd()
    {

        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        interstitialAd = new InterstitialAd(ADMOB_INTERSTITITIAL_ID);

        interstitialAd.OnAdClosed += OnInterstitialClosed;

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());
    }

    private void OnInterstitialClosed(System.Object sender, EventArgs args)
    {
        RequestAndLoadInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        if(interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
        else
        {
            RequestAndLoadInterstitialAd();
        }
    }
   
#endregion
}