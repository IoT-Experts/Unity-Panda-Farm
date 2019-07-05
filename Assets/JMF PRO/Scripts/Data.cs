using UnityEngine;
using System.Collections;
using System;
using CodeStage.AntiCheat;
using CodeStage.AntiCheat.ObscuredTypes;
public static class Data
{
    public static string keyScore = "score";
    public static string keyGio = "totalgio";
    public static string keyCoin = "totalcoin";
    public static string keyBinhThuoc = "totalbinhthuoc";
    public static string keyBinhXit = "totalbinhxit";
    public static string keyBanTay = "totalbantay";
    public static string keyBua = "totalbua";
    public static string keyDateTime = "time";
    public static string level = "level";
    public static int GetData(string key)
    {
        int a = ObscuredPrefs.GetInt(key);
        return a;
    }

    public static void UpdateData(string key, int amount)
    {
        int a = ObscuredPrefs.GetInt(key);
        a += amount;
        ObscuredPrefs.SetInt(key, a);
    }

    public static void RemoveData(string key, int amount)
    {
        int a = ObscuredPrefs.GetInt(key);
        a -= amount;
        ObscuredPrefs.SetInt(key, a);
    }

    public static DateTime GetDateTime()
    {
        if (string.IsNullOrEmpty(ObscuredPrefs.GetString(keyDateTime)))
        {
            ObscuredPrefs.SetString(keyDateTime, "4/5/2016 0:00:01 PM");
            
        }
        DateTime time = Convert.ToDateTime(ObscuredPrefs.GetString(keyDateTime));
        return time;
    }
    public static void SetDateTime()
    {
        ObscuredPrefs.SetString(keyDateTime, DateTime.Now.ToString());
    }

    public static void SetDateTimeDefault()
    {
        ObscuredPrefs.SetString(keyDateTime, "4/5/2016 0:00:01 PM");
    }

    public static void SetLevel(string level)
    {
        ObscuredPrefs.SetString("level", level);
    }

    public static int GetLevel()
    {
        int level = int.Parse(ObscuredPrefs.GetString("level").Substring(5));
        return level;
    }

}

