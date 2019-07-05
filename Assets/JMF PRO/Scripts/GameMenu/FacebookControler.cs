using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Newtonsoft.Json;
using UnityEngine.UI;
public class FacebookControler : MonoBehaviour
{
    public GameObject LayerFriendFacebook;
    public GameObject LayerSpinFacebook;
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        if (FB.IsLoggedIn)
        {
            gameObject.SetActive(false);
        }
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            //Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            //Time.timeScale = 1;
        }
    }


    void Start()
    {
        
    }
    public void LoginFB()
    {
        iTween.PunchScale(gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            gameObject.SetActive(false);
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            GetDataFacebook.IDFacebook = aToken.UserId;
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            LogInWithPublishPermissions();

        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void LogInWithPublishPermissions()
    {
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, AuthCallback1);
    }

    private void AuthCallback1(ILoginResult result)
    {
        gameObject.SetActive(false);
        if (Application.loadedLevelName == "GameMap")
        {
            GameObject getDatafacebook = GameObject.FindGameObjectWithTag("facebookcontroll");
            getDatafacebook.GetComponent<GetDataFacebook>().GetAvatar();
            LayerFriendFacebook.SetActive(true);
            AzureUILeaderboard azure = FindObjectOfType<AzureUILeaderboard>();
            azure.QueryListLevel(gameObject.name);
            LayerSpinFacebook.SetActive(false);
        }
    }


    public void PostScore()
    {
        // int score = ObscuredPrefs.GetInt("bestscore");
        int score = 100;
        FB.API("me/scores?score=" + score.ToString(), HttpMethod.POST, PostCallback);
    }

    private void PostCallback(IGraphResult result)
    {
        //Debug.Log(result.RawResult);
        //txtResult.text = result.RawResult;
        var a = JsonConvert.DeserializeObject<RootObject>(result.RawResult);
        if (a.success)
        {
            GetLeaderBoard();
        }
    }

    public void GetLeaderBoard()
    {
        FB.API("app/scores?fields=score", HttpMethod.GET, GetScoreCallback);
    }

    private void GetScoreCallback(IGraphResult result)
    {
        //txtResult.text = result.RawResult;
        // txt.text = result.RawResult;
        //LeaderBoard leaderBoard = FindObjectOfType<LeaderBoard>();
        //leaderBoard.BildingData(result.RawResult);
    }
    public class RootObject
    {
        public bool success { get; set; }
    }

}
