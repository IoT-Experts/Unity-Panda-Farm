using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using Facebook.Unity;

public class LayerSpinControll : MonoBehaviour
{
    public Button ButtonSpin;
    public Button ButtonClose;
    public Button ButtonYesBuySpin;
    public Button ButtonNoBuySpin;
    public Button ButtonBuyCoin;
    public GameObject Choose;
    public GameObject ShowCountTime;
    public GameObject SpinLoginFB;
    public GameObject LayerSpin;
    public GameObject PopUpBuySpin;
    public GameObject PopUpBuyCoin;
    public Text txtHours;
    public Text txtMinutes;
    public Text txtSecons;
    DateTime time1;
    DateTime time2;
    int countTime;
    int totalTime = 86400;
    float hours, minutes, seconds;
    float rotation;
    void Start()
    {
        SpinLoginFB.SetActive(false);
       // LayerSpin.SetActive(false);
        PopUpBuySpin.SetActive(false);
        PopUpBuyCoin.SetActive(false);
        //PlayerPrefs.SetInt(Data.keyCoin, 0);
        ButtonSpin.onClick.AddListener(() => ButtonSpinClick());
        ButtonClose.onClick.AddListener(() => ButtonCloseClick());
        ButtonYesBuySpin.onClick.AddListener(() => ButtonYesBuySpinClick());
        ButtonNoBuySpin.onClick.AddListener(() => ButtonNoBuySpinClick());
        ButtonBuyCoin.onClick.AddListener(() => ButtonBuyCoinClick());
        time1 = Data.GetDateTime();
       // ResetLayer();
    }

    void ButtonBuyCoinClick()
    {
        LayerSpin.SetActive(false);
        ControllerButtonMap controllerButtonMap = FindObjectOfType<ControllerButtonMap>();
        controllerButtonMap.btnShop();
        ResetLayer();
    }

    void ButtonYesBuySpinClick()
    {
        int coin = Data.GetData(Data.keyCoin);
        if (coin >= 5000)
        {
            Data.SetDateTimeDefault();
            time1 = Data.GetDateTime();
            Data.RemoveData(Data.keyCoin, 5000);
            ButtonSpinClick();
            PopUpBuySpin.SetActive(false);

        }
        else
        {
            PopUpBuyCoin.SetActive(true);
            // Debug.Log("bạn không đủ tiền để quay");
        }
    }

    void ButtonNoBuySpinClick()
    {
        PopUpBuySpin.SetActive(false);

    }
    // Update is called once per frame
    void ButtonSpinClick()
    {

        if (!FB.IsLoggedIn)
        {
            SpinLoginFB.SetActive(true);
        }
        else
        {
            if (countTime < 0)
            {
                RotationSpin();
            }
            else
            {
                PopUpBuySpin.SetActive(true);
            }
        }
    }

    void RotationSpin()
    {
        iTween.PunchScale(ButtonSpin.gameObject, new Vector3(0.5f, 0.5f), 0.7f);
        rotation = UnityEngine.Random.Range(7200, 12000);
        float a = rotation % 45;
        rotation -= a;
        iTween.Defaults.easeType = iTween.EaseType.easeInOutCirc;
        iTween.RotateTo(gameObject, new Vector3(0, 0, -rotation), 10);
        StartCoroutine(AddBoxForChoose());
        ButtonClose.gameObject.SetActive(false);
        ButtonSpin.onClick.RemoveAllListeners();
    }

    void Update()
    {
        time2 = DateTime.Now;
        var time = time2 - time1;
        var b = (int)time.TotalSeconds;
        countTime = totalTime - b;
        if (countTime > 0)
        {
            ShowCountTime.SetActive(true);
            hours = countTime / 3600;
            if (hours / 10 >= 1)
            {
                txtHours.text = hours.ToString() + ":";
            }
            else
            {
                txtHours.text = "0" + hours.ToString() + ":";
            }
            minutes = (countTime % 3600) / 60;
            if (minutes / 10 >= 1)
            {
                txtMinutes.text = minutes.ToString() + ":";
            }
            else
            {
                txtMinutes.text = "0" + minutes.ToString() + ":";
            }
            seconds = (countTime % 3600) % 60;
            if (seconds / 10 >= 1)
            {
                txtSecons.text = seconds.ToString();
            }
            else
            {
                txtSecons.text = "0" + seconds.ToString();
            }
        }
        else
        {
            ShowCountTime.SetActive(false);
        }

    }

    IEnumerator AddBoxForChoose()
    {
        yield return new WaitForSeconds(10.6f);
        Choose.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        Data.SetDateTime();
        time1 = Data.GetDateTime();
        yield return new WaitForSeconds(4f);
        ButtonClose.gameObject.SetActive(true);
        ButtonSpin.onClick.AddListener(() => ButtonSpinClick());
        Debug.Log(time1);
    }

    void ButtonCloseClick()
    {
        iTween.PunchScale(ButtonSpin.gameObject, new Vector3(0.5f, 0.5f), 0.7f);
        gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
        ResetLayer();
    }


    void ResetLayer()
    {
        SpinLoginFB.SetActive(false);
        LayerSpin.SetActive(false);
        PopUpBuySpin.SetActive(false);
        PopUpBuyCoin.SetActive(false);
    }
}
