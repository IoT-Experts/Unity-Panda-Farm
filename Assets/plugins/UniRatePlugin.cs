//
//	UniRatePlugin.cs
//  Created by Wang Wei(@onevcat) on 2013-7-15.
//
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UniRatePlugin {

	#if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern void _ShowPrompt(string title, string msg, string rateTitle, string canceltitle, string remindTitle);

	[DllImport ("__Internal")]
	private static extern string _GetAppStoreCountry();

	[DllImport ("__Internal")]
	private static extern string _GetApplicationName();

	[DllImport ("__Internal")]
	private static extern string _GetApplicationVersion();

	[DllImport ("__Internal")]
	private static extern string _GetApplicationBundleID();

	#elif UNITY_ANDROID
	private static AndroidJavaObject _uniRateInterface;
	#endif

	public static void ShowPrompt(string title, string msg, string rateTitle, string canceltitle, string remindTitle)
	{
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_ShowPrompt(title, msg, rateTitle, canceltitle, remindTitle);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			_uniRateInterface.Call("ShowPrompt",title,msg,rateTitle,canceltitle,remindTitle);
		}
#elif UNITY_WP8
        if (Application.platform == RuntimePlatform.WP8Player) {
            UniRateWP.Interface.ShowPrompt(title, msg, rateTitle, canceltitle, remindTitle);
        }
		#endif
	}

	public static string GetAppStoreCountry()
	{
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _GetAppStoreCountry();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			return "";
		}
		#endif
		return "";
	}

	public static string GetApplicationName()
	{
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _GetApplicationName();
		} 
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			return _uniRateInterface.Call<string>("GetAppName");
		}
        #elif UNITY_WP8
        if (Application.platform == RuntimePlatform.WP8Player)
        {
            return UniRateWP.Interface.GetAppName();
        }
		#endif
		return "";
	}

	public static string GetApplicationVersion()
	{
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _GetApplicationVersion();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			return _uniRateInterface.Call<string>("GetAppVersion");
		}
        #elif UNITY_WP8
        if (Application.platform == RuntimePlatform.WP8Player) {
            return UniRateWP.Interface.GetAppVersion();
        }
		#endif
		return "0";
	}

	public static string GetApplicationBundleID()
	{
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _GetApplicationBundleID();
		}
		#endif
		return "";
	}
	
	public static string GetPackageName()
	{
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			return _uniRateInterface.Call<string>("GetPackageName");
		}
		#endif
		return "";
	}

	public static void OpenAndroidRatePage(string rateURL)
	{
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			_uniRateInterface.Call("OpenRatePage",rateURL);
		} else {
            Debug.Log("The review page can only be opened on a real Android device.");
        }
        #endif
    }

    public static void OpenWPMarket()
    {
        #if UNITY_WP8
        if (Application.platform == RuntimePlatform.WP8Player) {
            UniRateWP.Interface.OpenWPMarket();
        } else {
            Debug.Log("The review page can only be opened on a real WP8 device.");
        }
        #endif
    }

	public static void InitUniRateAndroid() {
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (_uniRateInterface == null) {
				_uniRateInterface = new AndroidJavaObject("com.onevcat.unirate.UniRatePlugin");	
			}
		}
		#endif
	}
}
