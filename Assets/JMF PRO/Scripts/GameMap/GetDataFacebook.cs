using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
public class GetDataFacebook : MonoBehaviour
{
    // public Text text;
    public Image imageAvatar;
    public Image[] lstFriend;
    public Text[] lstName;
    public Text[] lstScore;
    public Sprite sprite;
    public GameObject LayerShowAvatarFriends;
    public static string IDFacebook;
    public static string UserFacebook;
    public static string Level;
    // Use this for initialization
    void Start()
    {

        if (Application.loadedLevelName == "GameMap")            

            
        {
            if (FB.IsLoggedIn)
            {
                GetInfoFB();
                GetAvatar();
                LayerShowAvatarFriends.SetActive(true);
                ClearData();
            }
        }
    }

    public void ClearData()
    {
        for (int i = 0; i < lstFriend.Length; i++)
        {
            lstFriend[i].sprite = sprite;
            lstName[i].text = "";
            lstScore[i].text = "";
        }
    }


    void GetOldLevel()
    {

    }
    public void GetInfoFB()
    {
        FB.API("me", HttpMethod.GET, GetInfoCallback);
    }

    private void GetInfoCallback(IGraphResult result)
    {
        var a = JsonConvert.DeserializeObject<User>(result.RawResult);
        IDFacebook = a.id;
        UserFacebook = a.name;
    }
    public void GetAvatar()
    {
        FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePhotoCallback);      
    }

    private void ProfilePhotoCallback(IGraphResult result)
    {
        if (result.Texture != null)
        {
            imageAvatar.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        }
    }


    public IEnumerator getAvatarFriend(List<string> id)
    {
        for (int i = 0; i < id.Count; i++)
        {
            string link = "https://graph.facebook.com/" + id[i] + "/picture?type=square";
            WWW getTexture = new WWW(link);
            yield return getTexture;
            if (getTexture.error == null)
            {
                lstFriend[i].sprite = Sprite.Create(getTexture.texture, new Rect(0, 0, getTexture.texture.width, getTexture.texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public class User
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Datum
    {
        public int score { get; set; }
        public User user { get; set; }
    }

    public class ListData
    {
        public List<Datum> data { get; set; }
    }


}
