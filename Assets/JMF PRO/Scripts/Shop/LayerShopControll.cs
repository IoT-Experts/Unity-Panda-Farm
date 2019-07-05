using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class LayerShopControll : MonoBehaviour
{
    public static LayerShopControll layerShopControll;
    public GameObject LayerCoin, LayerItem, LayerPopupBuyItem;
    public Button btnCloseLayer;
    public Button btnItem;
    public Button btnCoin;
    Sprite itemLayerOn, itemLayerOff, coinLayerOn, coinLayerOff;
    Vector3 scaleCurrent = new Vector3(1, 1);
    Vector3 scaleClick = new Vector3(1f, 1.1f);
    GameObject parent;

    void Start()
    {
        parent = gameObject;
        layerShopControll = this;
        btnCloseLayer.onClick.AddListener(() => btnClose());
        btnItem.onClick.AddListener(() => btnItemClick());
        btnCoin.onClick.AddListener(() => btnCoinClick());
        LayerItem.SetActive(true);
        LayerCoin.SetActive(false);
        itemLayerOn = Resources.Load<Sprite>("Textures/GameMap/item shop/btnItemOn");
        itemLayerOff = Resources.Load<Sprite>("Textures/GameMap/item shop/btnItemOff");
        coinLayerOn = Resources.Load<Sprite>("Textures/GameMap/item shop/btnCoinOn");
        coinLayerOff = Resources.Load<Sprite>("Textures/GameMap/item shop/btnCoinOff");
        btnItem.GetComponent<Image>().sprite = itemLayerOn;
        btnCoin.GetComponent<Image>().sprite = coinLayerOff;
        iTween.ScaleTo(btnItem.gameObject, scaleClick, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void btnClose()
    {
        iTween.Defaults.easeType = iTween.EaseType.linear;
        iTween.PunchScale(btnCloseLayer.gameObject, scaleClick, 0.5f);
        gameObject.GetComponent<Animator>().Play("LayerShopHide");
        if (Application.loadedLevelName == "GamePlay")
        {
            GP_TrayItem trayitem = FindObjectOfType<GP_TrayItem>();
            trayitem.showShop = false;
        }
    }

    void btnItemClick()
    {
        LayerItem.SetActive(true);
        LayerCoin.SetActive(false);
        btnItem.GetComponent<Image>().sprite = itemLayerOn;
        btnCoin.GetComponent<Image>().sprite = coinLayerOff;
        iTween.ScaleTo(btnCoin.gameObject, scaleCurrent, 0.5f);
        iTween.ScaleTo(btnItem.gameObject, scaleClick, 0.5f);
    }

    public void btnCoinClick()
    {
        LayerCoin.SetActive(true);
        LayerItem.SetActive(false);
        btnItem.GetComponent<Image>().sprite = itemLayerOff;
        btnCoin.GetComponent<Image>().sprite = coinLayerOn;
        iTween.ScaleTo(btnItem.gameObject, scaleCurrent, 0.5f);
        iTween.ScaleTo(btnCoin.gameObject, scaleClick, 0.5f);
    }

    public void ShowPopupBuy(string name)
    {
        GameObject a = Instantiate(LayerPopupBuyItem);
        a.transform.parent = parent.transform;
        a.transform.localScale = new Vector3(1, 1);
        a.transform.localPosition = new Vector3(0, 0, -10);
    }
}
