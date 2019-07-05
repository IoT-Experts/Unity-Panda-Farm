using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bitrave.Azure;
using UnityEngine.UI;

public class LayerMoreGame : MonoBehaviour
{

    private AzureMobileServices azure;
    [SerializeField]
    public string AzureEndPoint = "https://jetsoftgame.azure-mobile.net/"; // Your Connection URL

    [SerializeField]
    public string ApplicationKey = "afaWTcNSVpvjIOHmWpDazXgLsiTmcM51"; // Your API Key


    public List<moregame> _moregame = new List<moregame>();
    public GameObject Content;
    public GameObject ItemMoregame;
    int count;
    List<GameObject> listItem = new List<GameObject>();
    void Start()
    {
        LoginAzure();
        //PostData();
        QueryAllItem();
    }

    // Item to insert
    //private moregame _leaderboard = new moregame()
    //{
    //    LinkImage = "",
    //    Username = "",
    //    Package = ""

    //};

    // Item to update
    public moregame _selectedItem = new moregame()
    {
        LinkImage = "",
        Username = "",
        Package = ""
    };

    public void LoginAzure()
    {
        azure = new AzureMobileServices(AzureEndPoint, ApplicationKey);
    }


    void QueryAllItem()
    {
        _moregame.Clear();
        azure.Where<moregame>(p => p.Username != null, QueryCallback);
    }

    private void QueryCallback(AzureResponse<List<moregame>> obj)
    {
        var list = obj.ResponseData;
        for (int i = 0; i < list.Count; i++)
        {
            _moregame.Add(list[i]);
        }
    }

    public void BindingData()
    {
        listItem.Clear();
        for (int i = 0; i < _moregame.Count; i++)
        {
            if (_moregame[i].Package != Application.bundleIdentifier)
            {
                GameObject a = Instantiate(ItemMoregame) as GameObject;
                a.transform.SetParent(Content.transform,false);
                a.transform.GetChild(1).GetComponent<Text>().text = _moregame[i].Username;
                a.name = _moregame[i].Package;
                a.GetComponent<Button>().onClick.AddListener(() => OpenUrl(a.name));
                listItem.Add(a);
            }
        }
        StartCoroutine(GetIconGame());
    }

    public void RemoveData()
    {
        foreach (var item in listItem)
        {
            Destroy(item);
        }
        count = 0;
    }

    IEnumerator GetIconGame()
    {
        for (int i = 0; i < _moregame.Count; i++)
        {
            if (_moregame[i].Package != Application.bundleIdentifier)
            {
                string link = _moregame[i].LinkImage;
                WWW getTexture = new WWW(link);
                yield return getTexture;
                if (getTexture.error == null)
                {
                    if (listItem[count] != null)
                    {
                        Image image = listItem[count].transform.GetChild(0).GetComponent<Image>();
                        image.sprite = Sprite.Create(getTexture.texture, new Rect(0, 0, getTexture.texture.width, getTexture.texture.height), new Vector2(0.5f, 0.5f));
                    }
                }
                count++;
            }
        }
    }

    //void PostData()
    //{
    //    _selectedItem.Package = "com.jetsoftstudio.powercity";
    //    _selectedItem.LinkImage = "https://scontent-hkg3-1.xx.fbcdn.net/hphotos-xal1/v/t1.0-9/12928126_872720962837540_8561793864443251987_n.jpg?oh=c5b24f58d1a32028ba9383d7809f448b&oe=57B9843E";
    //    _selectedItem.Username = "Power City";
    //    azure.Insert<moregame>(_selectedItem);
    //}

    void OpenUrl(string package)
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + package);
    }


}
