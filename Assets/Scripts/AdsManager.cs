using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public Action onCloseInterstitial;

    private BannerView banner;
    private InterstitialAd interstitial;


    private void OnEnable()
    {
        MobileAds.Initialize(initStatus => { });
    }

    private void Start()
    {
        RequestBanner();
        RequestInterstitial();
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        banner = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);


        banner.OnAdLoaded += OnLoadBanner;
        banner.OnAdFailedToLoad+= OnFailedToLoadBanner;


        //real
        // AdRequest request = new AdRequest.Builder().Build();

        //test
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
            .Build();

        banner.LoadAd(request);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
            string adUnitId = "unexpected_platform";
#endif

        interstitial = new InterstitialAd(adUnitId);
        
        interstitial.OnAdLoaded += OnLoadInterstitial;
        interstitial.OnAdClosed += OnFinishInterstitial;
        interstitial.OnAdFailedToLoad += OnFailedToLoadInterstitial;

        //real
        // AdRequest request = new AdRequest.Builder().Build();

        //test
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
            .Build();

        interstitial.LoadAd(request);
    }

    private void DisplayBanner()
    {
        banner.Show();
    }

    private void DisplayInterstitial()
    {
        if(interstitial.IsLoaded())
            interstitial.Show();
    }


    private void OnLoadBanner(object sender, EventArgs args)
    {
        print("banner display");
        DisplayBanner();
    }

    private void OnLoadInterstitial(object sender, EventArgs args)
    {
        print("Loaded interstitial");
        DisplayInterstitial();
    }

    private void OnFinishInterstitial(object sender, EventArgs args)
    {
        if(onCloseInterstitial != null)
            onCloseInterstitial.Invoke();
    }

    private void OnFailedToLoadBanner(object sender, EventArgs args)
    {
        print("FAILED BANNER, TRYING AGAIN");
        RequestBanner();
    }

    private void OnFailedToLoadInterstitial(object sender, EventArgs args)
    {
        print("FAILED INTERSTITIAL, TRYING AGAIN");
        RequestInterstitial();
    }

    
}
