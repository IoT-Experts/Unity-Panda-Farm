using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CodeStage.AntiCheat.ObscuredTypes;
public class PopupBuyItem : MonoBehaviour
{

    public Text txtCountItem, txtThanhTien;
    public Button btnTang;
    public Button btnGiam;
    public Button buyItem;
    public Button closeLayer;
    public Button ButtonAskCoin;
    public Image imageItem;
    public GameObject buyItemComplete;
    public List<Sprite> lstSpriteItem;
    public GameObject effectBuyComplete;
    public GameObject LayerAskBuyCoin;
    int countItem = 1;
    int totalCoin;
    int tongtien;
    int giatien = 10000;


    void Start()
    {
        //ObscuredPrefs.SetInt("totalcoin",0);
        btnTang.onClick.AddListener(() => TangItemClick());
        btnGiam.onClick.AddListener(() => GiamItemClick());
        buyItem.onClick.AddListener(() => BuyItemClick());
        closeLayer.onClick.AddListener(() => CloseLayer());
        ButtonAskCoin.onClick.AddListener(() => ButtonAskCoinClick());
        LayerAskBuyCoin.SetActive(false);
        string name = ObscuredPrefs.GetString("item");
        switch (name)
        {
            case "binhthuoc":
                imageItem.sprite = lstSpriteItem[0];
                break;
            case "binhxit":
                imageItem.sprite = lstSpriteItem[1];
                break;
            case "gio":
                imageItem.sprite = lstSpriteItem[2];
                giatien = 2000;
                break;
            case "bantay":
                imageItem.sprite = lstSpriteItem[3];
                break;
            case "bua":
                imageItem.sprite = lstSpriteItem[4];
                break;
            default:
                break;
        }
        buyItemComplete.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        txtThanhTien.text = (countItem * giatien).ToString();
        txtCountItem.text = "+" + countItem.ToString();
    }

    void TangItemClick()
    {
        if (countItem < 99)
        {
            countItem++;

        }
    }

    void GiamItemClick()
    {
        if (countItem > 1)
        {
            countItem--;
        }
    }

    void BuyItemClick()
    {
        totalCoin = Data.GetData(Data.keyCoin);
        tongtien = countItem * giatien;
        if (totalCoin > tongtien)
        {
            buyItemComplete.SetActive(true);
            effectBuyComplete.SetActive(true);
            StartCoroutine(ResetEffect());
            StartCoroutine(HideBuyComplete());
            iTween.Defaults.easeType = iTween.EaseType.easeInBack;
            iTween.PunchScale(imageItem.gameObject, new Vector3(1f, 1f), 2);
            Data.RemoveData(Data.keyCoin, tongtien);

        }
        else
        {
            LayerAskBuyCoin.SetActive(true);
        }
    }

    void CloseLayer()
    {
        iTween.PunchScale(closeLayer.gameObject, new Vector3(0.5f, 0.5f), 1);
        gameObject.GetComponent<Animator>().Play("LayerPopupBuyItemHide");
        Destroy(gameObject, 0.5f);
    }

    void ButtonAskCoinClick()
    {
        iTween.PunchScale(ButtonAskCoin.gameObject, new Vector3(0.5f, 0.5f), 0.6f);
        LayerShopControll layerShop = FindObjectOfType<LayerShopControll>();
        layerShop.btnCoinClick();
        Destroy(gameObject, 0.6f);
    }
    IEnumerator ResetEffect()
    {
        yield return new WaitForSeconds(1.5f);
        effectBuyComplete.SetActive(false);
    }
    IEnumerator HideBuyComplete()
    {
        yield return new WaitForSeconds(2f);
        iTween.Stop(imageItem.gameObject);
        buyItemComplete.SetActive(false);       
        string name = ObscuredPrefs.GetString("item");
        string key = "total" + name;
        Data.UpdateData(key, countItem);      
        countItem = 1;
        if (Application.loadedLevelName == "GamePlay")
        {
            GP_TrayItem trayItem = FindObjectOfType<GP_TrayItem>();
            trayItem.GetDataItem();
            
        }
    }
}
