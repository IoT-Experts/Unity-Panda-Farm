using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

public class ChooseLevel : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => { btnClick(); });
    }

    void btnClick()
    {
        iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f), 0.3f);
        ControllerButtonMap controllerButtonMap = FindObjectOfType<ControllerButtonMap>();
        ReadDataMap readData = FindObjectOfType<ReadDataMap>();
        readData.ReadMission(gameObject.name);
        controllerButtonMap.btnChooseLevel();
        int level=int.Parse(gameObject.name.Substring(5));       
        ObscuredPrefs.SetInt("level", level);
        if (!string.IsNullOrEmpty(GetDataFacebook.IDFacebook))
        {
            GetDataFacebook getdatafacebook = FindObjectOfType<GetDataFacebook>();
            getdatafacebook.ClearData();
            AzureUILeaderboard azure = FindObjectOfType<AzureUILeaderboard>();
            azure.QueryListLevel(gameObject.name);
        }
        
       // admodads.bannerView.Show();

    }
}
