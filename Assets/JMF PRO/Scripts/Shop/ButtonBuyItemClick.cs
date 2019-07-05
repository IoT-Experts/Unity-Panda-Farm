using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
public class ButtonBuyItemClick : MonoBehaviour
{

    Button buttonItem;
    // Use this for initialization
    void Start()
    {
        buttonItem = GetComponent<Button>();
        buttonItem.onClick.AddListener(() => buttonClick());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void buttonClick()
    {
        iTween.PunchScale(gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        switch (gameObject.tag)
        {
            case "buyItem":
                LayerShopControll.layerShopControll.ShowPopupBuy(gameObject.name);
                ObscuredPrefs.SetString("item", gameObject.name);
                break;
            case "buyGold":
                BuyGold(gameObject.name);
                break;
        }
    }

    void BuyGold(string name)
    {
        InAppGame inApp = FindObjectOfType<InAppGame>();

        switch (name)
        {
            case "do099":
                inApp.BuyProductID(InAppGame.kProductIDConsumable099);
                break;
            case "do299":
                inApp.BuyProductID(InAppGame.kProductIDConsumable299);
                break;
            case "do499":
                inApp.BuyProductID(InAppGame.kProductIDConsumable499);
                break;
            case "do999":
                inApp.BuyProductID(InAppGame.kProductIDConsumable999);
                break;
            case "do4999":
                inApp.BuyProductID(InAppGame.kProductIDConsumable4999);
                break;
            case "do9999":
                inApp.BuyProductID(InAppGame.kProductIDConsumable9999);
                break;
            default:
                break;
        }
    }
}
