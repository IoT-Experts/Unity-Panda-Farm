using UnityEngine;
using System.Collections;
//using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using System;

public class admodads : MonoBehaviour
{


    public Button Box;
    GP_TrayBasket trayBasket;
    float time = 10;
    string idVungle = "56fa5a4502a1e38333000009";
    public GameObject GioTraThuong;
    public GameObject GamePanel;
    public static BannerView bannerView;
    private static InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;
    public bool rewardBasedEventHandlersSet;
    void Start()
    {
        Vungle.init(idVungle, "", "");
        trayBasket = FindObjectOfType<GP_TrayBasket>();
        Box.gameObject.SetActive(false);
        Box.onClick.AddListener(() => showads());
        check = false;
        rewardBasedVideo = RewardBasedVideoAd.Instance;
         RequestInterstitial();
        RequestBanner();        
    }

    public static void showfulladmob()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show(); 
        }
    }
    public static void RequestBanner()
    {
        if (bShow==false)
        {
            string adUnitId = "ca-app-pub-3680030919103879/5687944143";
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            bannerView.LoadAd(request);
            bannerView.Hide();            
            bShow = true;
        }       
    }

    public static void showbanneradmob()
    {
        bannerView.Show();
    }

    public static bool bShow;
    public static void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
            bShow = false;
            bannerView = null;
        }
    }
    private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs e)
    {

    }

    private void HandleRewardBasedVideoClosed(object sender, EventArgs e)
    {

    }

    private void HandleRewardBasedVideoStarted(object sender, EventArgs e)
    {

    }

    private void HandleRewardBasedVideoOpened(object sender, EventArgs e)
    {

    }

    private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        if (countads % 2 == 0)
        {
            ShowRewardedAd();
        }
        else
        {
            ShowVungleAds();
        }
        countads++;
    }

    private void HandleRewardBasedVideoLoaded(object sender, EventArgs e)
    {

    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        print("User rewarded with: " + amount.ToString() + " " + type);
        StartCoroutine(TraThuong(2));
        StartCoroutine(doitrathuong());
        congtrathuong = true;
        check = false;
    }
    private AdRequest createAdRequest()
    {
        return new AdRequest.Builder().Build();

    }
    private void RequestRewardBasedVideo()
    { 
        string adUnitId = "ca-app-pub-3680030919103879/3147790149";  
        RewardBasedVideoAd rewardBasedVideo = RewardBasedVideoAd.Instance;
        //  AdRequest request = new AdRequest.Builder().Build();
        AdRequest request = new AdRequest.Builder().Build();    
        rewardBasedVideo.LoadAd(request, adUnitId);
       rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;

    }
     public static void RequestInterstitial()
    {
        
        string adUnitId = "ca-app-pub-3680030919103879/5078521743";
        interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    IEnumerator TraThuong(int soluong)
    {
        for (int i = 0; i < soluong; i++)
        {
            GameObject a = Instantiate(GioTraThuong) as GameObject;
            yield return new WaitForSeconds(1f);
            Hashtable hh = new Hashtable();
            hh.Add("x", -0.2f);
            hh.Add("y", -0.3f);
            hh.Add("time", 1f);
            iTween.ShakePosition(GamePanel, hh);
            iTween.ShakePosition(a, hh);
            yield return new WaitForSeconds(1);
            StartCoroutine(trayBasket.AddBasket(1, a.transform.position, 1));
            Destroy(a, 1f);
        }
    }
    void showvideoAdmod()
    {
        if (!rewardBasedEventHandlersSet)
        {
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // has failed to load.
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // is opened.
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            // has started playing.
            rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            // has rewarded the user.

            // is closed.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            // is leaving the application.

            rewardBasedEventHandlersSet = true;
        }
    }
    public bool congtrathuong = false;
    void ShowVungleAds()
    {
        Vungle.playAd(true, "", 1);
        Vungle.onAdFinishedEvent += (args) =>
        {
            if (args.IsCompletedView)
            {
                if (congtrathuong == false)
                {
                    StartCoroutine(TraThuong(2));
                    StartCoroutine(doitrathuong());
                    congtrathuong = true;
                    check = false;
                }
            }
            else
            {
                check = false;
                time = 15;
            }
        };
        if (!Vungle.isAdvertAvailable())
        {
            check = false;
            ShowRewardedAd();
        }
    }

    IEnumerator doitrathuong()
    {
        yield return new WaitForSeconds(5f);
        congtrathuong = false;
    }
    public static int countads;
    public void showads()
    {
        Box.gameObject.SetActive(false);
        time = 5;
       // RequestRewardBasedVideo();
        //if (rewardBasedVideo.IsLoaded())
        //{
        //    rewardBasedVideo.Show();           
        //}
        //else
        //{
        //    RequestRewardBasedVideo();
        //    rewardBasedVideo.Show();
        //}
        if (countads % 2 == 0)
        {
            ShowRewardedAd();
        }
        else
        {
            ShowVungleAds();
        }
        countads++;
        check = false;
    }
    public string zoneId = "1052961";
    public int rewardQty = 250;
    public void ShowRewardedAd()
    {
        if (string.IsNullOrEmpty(zoneId)) zoneId = null;
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;
        Advertisement.Show(zoneId, options);
        check = false;
    }


    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:

                StartCoroutine(TraThuong(2));
                //              
                check = false;
                break;
            case ShowResult.Skipped:
                check = false;
                time = 15;

                break;
            case ShowResult.Failed:

                ShowVungleAds();
                check = false;
                break;
        }
    }


    // Update is called once per frame
    public float Timescheck = 120;
    public static bool check = false;
    void Update()
    {
        Timescheck -= Time.deltaTime;
        time -= Time.deltaTime;
        if (!check && time <= 0)
        {
            check = true;
            Box.gameObject.SetActive(true);
            iTween.PunchScale(Box.transform.GetChild(1).gameObject, iTween.Hash("x", 0.3f, "y", 0.3f, "time", 4, "looptype", iTween.LoopType.pingPong));
        }
        if (Timescheck <= 0)
        {
            if (bannerView != null)
            {
                bannerView.Hide();
            }            
        }
    }
}

