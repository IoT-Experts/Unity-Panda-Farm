using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using CodeStage.AntiCheat.ObscuredTypes;

public class ReadDataMap : MonoBehaviour
{

    //[HideInInspector]
    public List<GameObject> lstLevel;
    [HideInInspector]
    public List<GameObject> lstItemMission;

    public List<Sprite> lstSpriteButton;
    public List<Sprite> lstSpriteBug;
    public GameObject Avatar;
    public int totalLevel;
    public GameObject itemMission;
    public GameObject parentMission;
    public List<Sprite> lstSprite;
    public GameObject Content;
    static int count;

    // mission
    public Text txtScore;
    public Text txtLevel;
    // Use this for initialization

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        //ObscuredPrefs.DeleteKey("DataMap");
        count++;
        CreateListLevel();
        CheckDataMap();
    }

    private void CreateListLevel()
    {
        for (int i = 1; i <= totalLevel; i++)
        {
            GameObject a = GameObject.Find("LevelParent" + (i));
            a.transform.GetChild(0).name = "Level" + i;
            a.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            lstLevel.Add(a);
        }
    }

    // Update is called once per frame  
    void CheckDataMap()
    {
        if (string.IsNullOrEmpty(ObscuredPrefs.GetString("DataMap")))
        {
            CreateData();
        }
        else
        {
            ReadData();
        }
    }
    void CreateData()
    {
        Levemapmanager l = new Levemapmanager();
        for (int i = 0; i < lstLevel.Count; i++)
        {
            LevelProperty levelmap = new LevelProperty();
            if (i <= level)
            {
                levelmap.Stt = i;
                levelmap.KeyLock = 1;
                levelmap.Star = 0;
            }
            else
            {
                levelmap.Stt = i;
                levelmap.KeyLock = 0;
                levelmap.Star = 0;
            }
            l.LevelMaps.Add(levelmap);
        }
        var obj1 = JsonConvert.SerializeObject(l);
        ObscuredPrefs.SetString("DataMap", obj1);
        ReadData();
    }

    //Key Lock
    // -1:Complete
    // 0:Lock
    // 1:Unlock
    int level;
    void ReadData()
    {
        string dataMap = ObscuredPrefs.GetString("DataMap");
        var objdata = JsonConvert.DeserializeObject<Levemapmanager>(dataMap);

        if (objdata.LevelMaps.Count < totalLevel)
        {
            level = objdata.LevelMaps.FindIndex(p => p.KeyLock == 1);
            ObscuredPrefs.SetString("DataMap", "");
            CheckDataMap();
            return;
        }
        else
        {
            for (int i = 0; i < objdata.LevelMaps.Count; i++)
            {
                int keylock = objdata.LevelMaps[i].KeyLock;
                Image[] a = lstLevel[i].GetComponentsInChildren<Image>();

                switch (keylock)
                {
                    case -1:
                        lstLevel[i].GetComponent<Animator>().enabled = false;
                        lstLevel[i].transform.GetChild(0).GetComponent<Button>().enabled = true;

                        a[0].sprite = lstSpriteButton[0];
                        lstLevel[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().enabled = true;
                        break;
                    case 0:
                        lstLevel[i].GetComponent<Animator>().enabled = false;
                        lstLevel[i].transform.GetChild(0).GetComponent<Button>().enabled = false;
                        a[0].sprite = lstSpriteButton[1];
                        lstLevel[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().enabled = false;
                        break;
                    case 1:
                        lstLevel[i].GetComponent<Animator>().enabled = true;
                        lstLevel[i].transform.GetChild(0).GetComponent<Button>().enabled = true;
                        a[0].sprite = lstSpriteButton[0];
                        lstLevel[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().enabled = true;

                        if (count > 1 && i > 0)
                        {
                            Avatar.transform.position = lstLevel[i - 1].transform.position;
                        }
                        else
                        {
                            Avatar.transform.position = lstLevel[0].transform.position;
                        }
                        MoveAvatar(lstLevel[i].transform.position);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void MoveAvatar(Vector3 position)
    {

        Hashtable hh = new Hashtable();
        hh.Add("time", 0.5f);
        hh.Add("position", position);
        iTween.MoveTo(Avatar, hh);

    }

    public void ReadMission(string name)
    {
        if (lstItemMission.Count > 0)
        {
            foreach (var item in lstItemMission)
            {
                Destroy(item);
            }
        }
        txtLevel.text = name;
        TextAsset file = (TextAsset)Resources.Load("Level/" + name);
        var obj = JsonConvert.DeserializeObject<GP_ClassData>(file.text);
        if (obj.taregtScore && !obj.targetFruit && !obj.targetBug)
        {
            txtScore.text = obj.score1.ToString();
        }

        else if (obj.targetFruit && !obj.targetBug)
        {
            txtScore.text = "";
            parentMission.SetActive(true);
            for (int i = 0; i < obj.lstMissionFruitAmout.Count; i++)
            {
                var index = lstSprite.FindIndex(x => x.name == obj.lstMissionFruitAmout[i].name);
                GameObject a = Instantiate(itemMission) as GameObject;
                //a.transform.parent = parentMission.transform;
                //a.transform.localScale = new Vector3(1, 1, 1);
                a.transform.SetParent(parentMission.transform, false);
                lstItemMission.Add(a);
                a.transform.GetChild(1).GetComponent<Image>().sprite = lstSprite[index];
                a.transform.GetChild(0).GetComponent<Text>().text = obj.lstMissionFruitAmout[i].amount.ToString() + "/" + obj.lstMissionFruitAmout[i].amount.ToString();
            }
        }

        else if (!obj.targetFruit && obj.targetBug)
        {
            txtScore.text = "";
            parentMission.SetActive(true);
            for (int i = 0; i < obj.soluongSau.Count; i++)
            {
                GameObject a = Instantiate(itemMission) as GameObject;
                a.transform.parent = parentMission.transform;
                a.transform.localScale = new Vector3(1, 1, 1);
                lstItemMission.Add(a);
                a.transform.GetChild(1).GetComponent<Image>().sprite = lstSpriteBug[i];
                a.transform.GetChild(0).GetComponent<Text>().text = obj.soluongSau[i].ToString() + "/" + obj.soluongSau[i].ToString();
            }
        }
    }
}
