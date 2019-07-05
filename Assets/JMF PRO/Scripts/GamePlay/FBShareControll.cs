using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using System;
using System.IO;
public class FBShareControll : MonoBehaviour
{
    public Texture2D screenTexture;
    public Button ButtonYes;
    public Button ButtonNo;
    // Use this for initialization
    void Start()
    {
        ButtonYes.onClick.AddListener(() => ButtonYesClick());
        ButtonNo.onClick.AddListener(() => ButtonNoClick());
    }

    // Update is called once per frame    
    void ButtonYesClick()
    {
        if (FB.IsLoggedIn)
        {
            Share();
            //StartCoroutine(ShareImageShot());
        }
    }



    void ButtonNoClick()
    {
        gameObject.SetActive(false);
    }

    public void Share()
    {
        FB.ShareLink(
    new Uri("https://www.facebook.com/Panda-Fruit-Farm-493542387510164/?skip_nax_wizard=true&__mref=message_bubble"),
    callback: ShareCallback
   );
    }


    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
            gameObject.SetActive(false);
        }
    }
   
}
   
