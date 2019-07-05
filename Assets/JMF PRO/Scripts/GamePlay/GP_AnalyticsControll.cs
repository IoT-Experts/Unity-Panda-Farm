using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class GP_AnalyticsControll : MonoBehaviour
{

    public GoogleAnalyticsV4 googleAnalytics;
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        googleAnalytics.StartSession();

        if (GoogleAnalyticsV4.instance)
        {
            GoogleAnalyticsV4.instance.LogScreen("GamePlay  " + ObscuredPrefs.GetString("level"));

        }
    }

}
