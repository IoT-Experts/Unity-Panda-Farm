using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

public class GP_TrayItem : MonoBehaviour
{
    GameObject ObjectBua, ObjectBantay;
    Animator anim;
    bool showTrayItem;
    public Button ButtonBinhthuoc;
    public Button ButtonBinhxit;
    public Button ButtonBantay;
    public Button ButtonBua;
    public GameObject GameManagerPanel;
    public GameObject LayerShop;
    public GameObject CanvasParent;
    [HideInInspector]
    public bool binhthuocClicked, binhxitClicked, bantayClicked, buaClicked;
    [HideInInspector]
    public bool showShop;
    [HideInInspector]
    public int countInput;
    [HideInInspector]
    public int[] pos1 = new int[2];
    [HideInInspector]
    public int[] pos2 = new int[2];
    public List<Sprite> lstSprite;
    public int countBinhThuoc = 3, countBinhXit = 3, countBanTay = 3, countBua = 3;
    GameObject bantayCurrent;
    void Start()
    {
        bantayCurrent = null;
        anim = GetComponent<Animator>();

        ButtonBinhthuoc.onClick.AddListener(() => { BinhThuoc(); });
        ButtonBinhxit.onClick.AddListener(() => { BinhXit(); });
        ButtonBantay.onClick.AddListener(() => { BanTay(); });
        ButtonBua.onClick.AddListener(() => { Bua(); });
        ObjectBua = Resources.Load<GameObject>("Prefabs/TrayItem/bua");
        ObjectBantay = Resources.Load<GameObject>("Prefabs/TrayItem/bantay");
        GetDataItem();
    }  

    public void GetDataItem()
    {       
        countBinhThuoc = Data.GetData(Data.keyBinhThuoc);
        countBinhXit = Data.GetData(Data.keyBinhXit);
        countBanTay = Data.GetData(Data.keyBanTay);
        countBua = Data.GetData(Data.keyBua);

        ButtonBinhthuoc.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = countBinhThuoc.ToString();
        ButtonBinhxit.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = countBinhXit.ToString();
        ButtonBantay.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = countBanTay.ToString();
        ButtonBua.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = countBua.ToString();
    }

    public void ButtonClick()
    {
        if (!showTrayItem)
        {
            showTrayItem = true;
            anim.Play("TrayItemShow");
        }
        else
        {
            showTrayItem = false;
            anim.Play("TrayItemHide");
        }


        AllButtonNormal();
    }


    public void AllButtonNormal()
    {
        if (bantayCurrent != null)
        {
            Destroy(bantayCurrent);
        }
        countInput = 0;
        binhthuocClicked = binhxitClicked = bantayClicked = buaClicked = false;

        ButtonBinhthuoc.GetComponent<Image>().sprite = lstSprite[0];
        ButtonBinhxit.GetComponent<Image>().sprite = lstSprite[1];
        ButtonBantay.GetComponent<Image>().sprite = lstSprite[2];
        ButtonBua.GetComponent<Image>().sprite = lstSprite[3];

        ButtonBinhthuoc.GetComponent<Button>().enabled = true;
        ButtonBinhxit.GetComponent<Button>().enabled = true;
        ButtonBantay.GetComponent<Button>().enabled = true;
        ButtonBua.GetComponent<Button>().enabled = true;

        ButtonBinhthuoc.transform.GetChild(1).gameObject.SetActive(false);
        ButtonBinhxit.transform.GetChild(1).gameObject.SetActive(false);
        ButtonBantay.transform.GetChild(1).gameObject.SetActive(false);
        ButtonBua.transform.GetChild(1).gameObject.SetActive(false);
    }

    void BinhThuoc()
    {
        if (countBinhThuoc != 0)
        {
            if (!binhthuocClicked)
            {
                binhthuocClicked = true;
                OpenItem(ButtonBinhthuoc);
                LockItem(ButtonBinhxit);
                LockItem(ButtonBantay);
                LockItem(ButtonBua);
            }
            else
            {
                AllButtonNormal();
            }
        }
        else
        {
            ObscuredPrefs.SetString("item", ButtonBinhthuoc.name);
            ShowLayerShop();
        }
    }

    void BinhXit()
    {
        if (countBinhXit != 0)
        {
            if (!binhxitClicked)
            {
                binhxitClicked = true;
                OpenItem(ButtonBinhxit);
                LockItem(ButtonBinhthuoc);
                LockItem(ButtonBantay);
                LockItem(ButtonBua);
            }
            else
            {
                AllButtonNormal();
            }

        }
        else
        {
            ObscuredPrefs.SetString("item", ButtonBinhxit.name);
            ShowLayerShop();
        }
    }

    public void BanTay()
    {
        if (countBanTay != 0)
        {
            if (!bantayClicked)
            {
                bantayClicked = true;
                bantayCurrent = Instantiate(ObjectBantay);
                bantayCurrent.transform.position = new Vector3(ButtonBantay.transform.position.x, ButtonBantay.transform.position.y + 1, -1);
                LockItem(ButtonBinhthuoc);
                LockItem(ButtonBinhxit);
                LockItem(ButtonBua);
            }
            else
            {
                AllButtonNormal();
            }
        }
        else
        {
            ObscuredPrefs.SetString("item", ButtonBantay.name);
            ShowLayerShop();
        }
    }

    void Bua()
    {
        if (countBua != 0)
        {
            if (!buaClicked)
            {
                buaClicked = true;

                OpenItem(ButtonBua);

                LockItem(ButtonBinhthuoc);
                LockItem(ButtonBinhxit);
                LockItem(ButtonBantay);
            }
            else
            {
                AllButtonNormal();
            }
        }
        else
        {
            ObscuredPrefs.SetString("item", ButtonBua.name);
            ShowLayerShop();
        }

    }

    void LockItem(Button button)
    {
        button.GetComponent<Image>().sprite = lstSprite[4];
        button.GetComponent<Button>().enabled = false;
    }

    void OpenItem(Button button)
    {
        button.transform.GetChild(1).gameObject.SetActive(true);
    }
    public IEnumerator BuaPlay(Vector3 position)
    {
        ButtonBinhxit.transform.GetChild(1).gameObject.SetActive(false);
        GameObject b = Instantiate(ObjectBua, new Vector3(position.x, position.y, -1), Quaternion.identity) as GameObject;
        Destroy(b, 1f);
        yield return new WaitForSeconds(1);
        Hashtable hh = new Hashtable();
        hh.Add("x", -0.2f);
        hh.Add("y", -0.3f);
        hh.Add("time", 0.5f);
        iTween.ShakePosition(GameManagerPanel, hh);
    }

    public IEnumerator BanTayPlay(Vector3 position)
    {
        iTween.MoveTo(bantayCurrent, new Vector3(position.x, position.y, -10), 0.5f);
        yield return new WaitForSeconds(0.5f);
        if (bantayCurrent != null)
        {
            bantayCurrent.GetComponent<Animator>().Play("bantayAnim");
        }
    }

    void ShowLayerShop()
    {
        GameObject a = Instantiate(LayerShop) as GameObject;
        a.transform.parent = CanvasParent.transform;
        a.transform.localScale = new Vector3(1, 1);
        CanvasParent.GetComponent<Canvas>().sortingOrder = 0;
        showShop = true;
    }
}
