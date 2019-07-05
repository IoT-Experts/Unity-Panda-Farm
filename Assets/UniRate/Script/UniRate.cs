//
//	UniRate.cs
//  Created by Wang Wei(@onevcat) on 2013-7-15.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The core class of UniRate. UniRate is a drop-in solution for rating prompt in Unity3D mobile game for iOS and Android.
/// </summary>
public class UniRate : MonoBehaviour {

	//Delegate and event
	public delegate void OnUniRateFaildDelegate(UniRate.Error error);
	public delegate void OnDetectAppUpdatedDelegate();
	public delegate bool ShouldUniRatePromptForRatingDelegate();
	public delegate void OnPromptedForRatingDelegate();
	public delegate void OnUserAttemptToRateDelegate();
	public delegate void OnUserDeclinedToRateDelegate();
	public delegate void OnUserWantReminderToRateDelegate();
	public delegate bool ShouldUniRateOpenRatePageDelegate();

	/// <summary>
	/// Occurs when there is an error during getting app info. A UniRate.Error will be sent.
	/// </summary>
	public event OnUniRateFaildDelegate OnUniRateFaild;

	/// <summary>
	/// Occurs when the app is updated. All count (used count, days count, etc) was reseted.
	/// </summary>
	public event OnDetectAppUpdatedDelegate OnDetectAppUpdated;

	/// <summary>
	/// UniRate asks if it should pop a prompt when it wants to show a prompt.
	/// </summary>
	public event ShouldUniRatePromptForRatingDelegate ShouldUniRatePromptForRating;

	/// <summary>
	/// Occurs when it is the time to prompt for rating and UniRate is going to make a prompt alert view(dialog).
	/// </summary>
	public event OnPromptedForRatingDelegate OnPromptedForRating;

	/// <summary>
	/// Occurs when user tapped "Rate" button in the prompt alert.
	/// </summary>
	public event OnUserAttemptToRateDelegate OnUserAttemptToRate;

	/// <summary>
	/// Occurs when user tapped "Cancel" button in the prompt alert.
	/// </summary>
	public event OnUserDeclinedToRateDelegate OnUserDeclinedToRate;

	/// <summary>
	/// Occurs when user tapped "Remind" button in the prompt alert.
	/// </summary>
	public event OnUserWantReminderToRateDelegate OnUserWantReminderToRate;

	/// <summary>
	/// UniRate asks if it should open the rate page when user clicked the Rate button.
	/// </summary>
	public event ShouldUniRateOpenRatePageDelegate ShouldUniRateOpenRatePage;

	#region Public

	/// <summary>
	/// The AppStore ID for the app. This is needed to be set only if you want specify your app id youself.
	/// Normally, UniRate will try to get your app id by query from Apple, so you are likely to not set it. 
	/// </summary>
	public int appStoreID;

	/// <summary>
	/// The market package name of Android app. This is needed to be set only if you want specify your app id youself.
	/// Normally, UniRate will try to get your market package name from the package info, so you are likely to not set it. 
	/// </summary>
	public string marketPackageName;

	/// <summary>
	/// The market you want UniRate to go.
	/// You can select from Google Play Market or Amazon Appstore. UniRate will handle the correct rating url for the market you selected.
	/// </summary>
	public UniRateMarketType marketType;

	//application details - these are set automatically
	/// <summary>
	/// The AppStore country code. This is needed to be set only if your app is not available in worldwide.
	/// Normally, UniRate will try to get the country code from user's phone setting and use it in querying app info
	/// You are likely to not set it yourself expect you are sure what to do. 
	/// </summary>
	public string appStoreCountry;

	/// <summary>
	/// The name of the application. This is needed to be set only if you want specify your app name showed in prompt different from your app's real name.
	/// Normally, UniRate will try to get the app name form the app's info, so you are likely to not set it. 
	/// </summary>
	public string applicationName;

	/// <summary>
	/// The application version. Each new version will reset the use counting. 
	/// Normally, UniRate will try to get the app version form the app's info, so you are likely to not set it. 
	/// </summary>
	public string applicationVersion;

	/// <summary>
	/// The application bundle ID for iOS app, which is in a form of "com.companyName.productionName".
	/// This is needed to be set only if you want specify your app bundle ID.
	/// Normally, UniRate will try to get the app bundle ID form the app's info, so you are likely to not set it. 
	/// </summary>
	public string applicationBundleID;

	//usage settings - these have sensible defaults
	/// <summary>
	/// The uses count until prompt. UniRate will not prompt when <see cref="usesCount"/> less than it.
	/// </summary>
	public int usesUntilPrompt = 10;

	/// <summary>
	/// The events count until prompt. UniRate will not prompt when <see cref="eventCount"/> less than it.
	/// </summary>
	public int eventsUntilPrompt = 0;

	/// <summary>
	/// The days until prompt. UniRate will not prompt when <see cref="usedDays"/> less than it.
	/// </summary>
	public float daysUntilPrompt = 3.0f;

	/// <summary>
	/// The uses per week for prompt. UniRate will not prompt when <see cref="usesPerWeek"/> less than it.
	/// </summary>
	public float usesPerWeekForPrompt = 0.0f;

	/// <summary>
	/// The remind period. When user required a remind, UniRate will prompt again after this period of time (day).
	/// </summary>
	public float remindPeriod = 1.0f;

	//message text, you may wish to customise these
	/// <summary>
	/// The title of prompt alert. You may wish to customise this.
	/// </summary>
	public string messageTitle = "";

	/// <summary>
	/// Gets the <see cref="messageTitle"/>. If not set, UniRate will use the default value.
	/// </summary>
	/// <value>The message title you set. Default value is localized string for "Rate {<see cref="applicationName"/>}"</value>
	public string MessageTitle { 
		get { 
			return (string.IsNullOrEmpty(messageTitle)) ? GetLocalizedMessageTitle() : messageTitle; 
		} 
	}

	/// <summary>
	/// The message of prompt alert. You may wish to customise this.
	/// </summary>
	public string message = "";

	/// <summary>
	/// Gets the <see cref="message"/>. If not set, UniRate will use the default value.
	/// </summary>
	/// <value>The message body you set. Default value is localized string for "If you enjoy {<see cref="applicationName"/>}, would you mind taking a moment to rate it? It will not take more than a minute. Thanks for your support!"</value>
	public string Message {
		get {
			return (string.IsNullOrEmpty(message)) ? GetLocalizedMessage() : message; 
		}
	}

	/// <summary>
	/// The cancel button label of prompt alert. You may wish to customise this.
	/// </summary>
	public string cancelButtonLabel = "";

	/// <summary>
	/// Gets the <see cref="cancelButtonLabel"/>. If not set, UniRate will use the default value.
	/// </summary>
	/// <value>The cancel button text. Default value is localized string for "No, thanks"</value>
	public string CancelButtonLabel {
		get {
			return (string.IsNullOrEmpty(cancelButtonLabel)) ? GetLocalizedCancelButtonLabel() : cancelButtonLabel; 
		}
	}

	/// <summary>
	/// The remind button label of prompt alert. You may wish to customise this.
	/// </summary>
	public string remindButtonLabel = "";

	/// <summary>
	/// Gets the <see cref="remindButtonLabel"/>. If not set, UniRate will use the default value.
	/// </summary>
	/// <value>The remind button text. Default value is localized string for "Remind me later"</value>
	public string RemindButtonLabel {
		get {
			return (string.IsNullOrEmpty(remindButtonLabel)) ? GetLocalizedRemindButtonLabel() : remindButtonLabel; 
		}
	}

	/// <summary>
	/// The rate button label of prompt alert. You may wish to customise this.
	/// </summary>
	public string rateButtonLabel = "";

	/// <summary>
	/// Gets the <see cref="rateButtonLabel"/>. If not set, UniRate will use the default value.
	/// </summary>
	/// <value>The rate button text. Default value is localized string for "Rate it now"</value>
	public string RateButtonLabel {
		get {
			return (string.IsNullOrEmpty(rateButtonLabel)) ? GetLocalizedRateButtonLabel() : rateButtonLabel; 
		}
	}

	//debugging and prompt overrides
	/// <summary>
	/// If true, only prompt if the running version is the latest version in AppStore.
	/// This is only available in iOS. Because there is no officail version check API for Android market
	/// </summary>
	public bool onlyPromptIfLatestVersion = true;

	/// <summary>
	/// If ture, each new version will count and prompts for rating.
	/// </summary>
	public bool promptAgainForEachNewVersion = true;

	/// <summary>
	/// If ture, UniRate will try to prompt at app launch finished if all conditions are met.
	/// </summary>
	public bool promptAtLaunch = true;

	/// <summary>
	/// If ture, UniRate will not pop up a system style prompt alert view(dialog).
	/// If you set it to ture, you should implement the <see cref="OnPromptedForRating"/> event and send the button tap message back to UniRate for properly handle.
	/// More about customized prompt view, see the demo of UniRateEventHandler's OnPromptedForRating method.
	/// </summary>
	public bool useCustomizedPromptView = false;

	/// <summary>
	/// If true, UniRate will use auto localized strings for prompt title, body, and buttons text.
	/// UniRate will use the value of Application.systemLanguage to decide which language to
	/// display. Not all languages are avalible. See the UniRateLocalizationStrings file for more. 
	/// </summary>
	public bool autoLocalization = true;

	/// <summary>
	/// The preview mode. If true, UniRate will prompt every time. You can use it for debugging. Remember to turn it off when release!
	/// </summary>
	public bool previewMode = false;
	
	public enum Error {
		BundleIdDoesNotMatchAppStore,
		AppNotFoundOnAppStore,
		NotTheLatestVersion,
		NetworkError,
	}

	//advanced properties for implementing custom behaviour
	[SerializeField]
	private string _ratingIOSURL;

	/// <summary>
	/// Gets the iOS rating URL. In most case it will be generated automaticlly by your setting. If you set _ratingIOSURL yourself, the set value will be returned.
	/// </summary>
	/// <value>The rating iOS URL.</value>
	public string RatingIOSURL {
		get {
			if (!string.IsNullOrEmpty(_ratingIOSURL)) {
				return _ratingIOSURL;
			}
			if (appStoreID == 0) {
				Debug.LogWarning("UniRate does not find your App Store ID");
			}
			return (this.iOSVersion >= 7.0f) ? kUniRateiOS7AppStoreURLFormat + appStoreID : 
											kUniRateiOSAppStoreURLFormat + appStoreID;
		}
	}

	[SerializeField]
	private string _ratingAndroidURL;
	/// <summary>
	/// Gets the Android rating URL. In most case it will be generated automaticlly by your setting. If you set _ratingAndroidURL yourself, the set value will be returned.
	/// </summary>
	/// <value>The rating Android URL.</value>
	public string RatingAndroidURL {
		get {
			if (!string.IsNullOrEmpty(_ratingAndroidURL)) {
				return _ratingAndroidURL;
			}
			string baseUrl = "";
			switch (marketType) {
				case UniRateMarketType.GooglePlay: baseUrl = kUniRateAndroidMarketURLFormat; break;
				case UniRateMarketType.AmazonAppstore: baseUrl = kUniRateAmazonAppstoreURLFormat; break;
				default: break;
			}
			return baseUrl + marketPackageName;
		}
	}

	/// <summary>
	/// Gets or sets the time when current version is first used.
	/// </summary>
	/// <value>The DateTime when first used.</value>
	public DateTime firstUsed {
		get {
			return UniRatePlayerPrefs.GetDate(kUniRateFirstUsedKey);
		}
		set {
			UniRatePlayerPrefs.SetDate(kUniRateFirstUsedKey, value);
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets or sets the time when last reminded happened.
	/// </summary>
	/// <value>The DateTime when last remind.</value>
	public DateTime lastReminded {
		get {
			return UniRatePlayerPrefs.GetDate(kUniRateLastRemindedKey);
		}
		set {
			UniRatePlayerPrefs.SetDate(kUniRateLastRemindedKey, value);
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets or sets the uses count. Every time the user open app (fresh start or switch from background), this count will be increased by 1.
	/// </summary>
	/// <value>The uses count.</value>
	public int usesCount {
		get {
			return PlayerPrefs.GetInt(kUniRateUseCountKey);
		}
		set {
			PlayerPrefs.SetInt(kUniRateUseCountKey, value);
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets or sets the event count. You can record the event by calling <see cref="LogEvent"/>
	/// </summary>
	/// <value>The event count.</value>
	public int eventCount {
		get {
			return PlayerPrefs.GetInt(kUniRateEventCountKey);
		}
		set {
			PlayerPrefs.SetInt(kUniRateEventCountKey, value);
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets the uses per week.
	/// </summary>
	/// <value>The uses per week.</value>
	public float usesPerWeek {
		get {
			return (float) (this.usesCount / ((DateTime.Now - this.firstUsed).TotalSeconds / SECONDS_IN_A_WEEK));
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether rating prompt for this version is declined.
	/// </summary>
	/// <value><c>true</c> if declined this version; otherwise, <c>false</c>.</value>
	public bool declinedThisVersion {
		get {
			if (!string.IsNullOrEmpty(applicationVersion)) {
				return string.Equals(PlayerPrefs.GetString(kUniRateDeclinedVersionKey), applicationVersion);
			}
			return false;
		}
		set {
			PlayerPrefs.SetString(kUniRateDeclinedVersionKey, value ? applicationVersion : "");
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets a value indicating whether rating prompt for any version is declined.
	/// </summary>
	/// <value><c>true</c> if declined any version; otherwise, <c>false</c>.</value>
	public bool declinedAnyVersion {
		get {
			return (string.IsNullOrEmpty(PlayerPrefs.GetString(kUniRateDeclinedVersionKey)) == false);
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this version is already rated.
	/// </summary>
	/// <value><c>true</c> if this version rated; otherwise, <c>false</c>.</value>
	public bool ratedThisVersion {
		get {
			return (PlayerPrefs.GetInt(kUniRateRatedVersionKey) == 1);
		}
		set {
			PlayerPrefs.SetInt(kUniRateRatedVersionKey, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	/// <summary>
	/// Gets a value indicating whether any version of the app is already rated.
	/// </summary>
	/// <value><c>true</c> if any version rated; otherwise, <c>false</c>.</value>
	public bool ratedAnyVersion {
		get {
			return (string.IsNullOrEmpty(PlayerPrefs.GetString(kUniRateRatedVersionKey)) == false);
		}
	}

	/// <summary>
	/// Gets the used days.
	/// </summary>
	/// <value>The used days.</value>
	public float usedDays {
		get {
			return (float)(DateTime.Now - this.firstUsed).TotalSeconds / SECONDS_IN_A_DAY;
		}
	}

	/// <summary>
	/// Gets a value indicating whether the user want to remind and UniRate is waiting for next prompt time.
	/// </summary>
	/// <value><c>true</c> if waiting waiting for next prompt time; otherwise, <c>false</c>.</value>
	public bool waitingByRemindLater {
		get {
			return (this.lastReminded != DateTime.MaxValue);
		}
	}

	/// <summary>
	/// Gets the left remind days.
	/// </summary>
	/// <value>The days until next prompt.</value>
	public float leftRemindDays {
		get {
			return (float)(remindPeriod - (DateTime.Now - this.lastReminded).TotalSeconds / SECONDS_IN_A_DAY);
		}
	}
	#endregion

	#region Private
	private const string kUniRateRatedVersionKey = @"UniRateRatedVersionChecked";
	private const string kUniRateDeclinedVersionKey = @"UniRateDeclinedVersion";
	private const string kUniRateLastRemindedKey = @"UniRateLastReminded";
	private const string kUniRateLastVersionUsedKey = @"UniRateLastVersionUsed";
	private const string kUniRateFirstUsedKey = @"UniRateFirstUsed";
	private const string kUniRateUseCountKey = @"UniRateUseCount";
	private const string kUniRateEventCountKey = @"UniRateEventCount";
	
	private const string kUniRateAppLookupURLFormat = @"http://itunes.apple.com/{0}lookup";

	private const string kUniRateiOSAppStoreURLFormat = @"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=";
	private const string kUniRateiOS7AppStoreURLFormat = @"itms-apps://itunes.apple.com/app/id";
	
	private const string kUniRateAndroidMarketURLFormat = @"market://details?id=";
	private const string kUniRateAmazonAppstoreURLFormat = @"amzn://apps/android?p=";

	private const string kDefaultTitle = "Rate {0}";
	private const string kDefaultMessage = "If you enjoy {0}, would you mind taking a moment to rate it? It will not take more than a minute. Thanks for your support!";
	private const string kDefaultCancelBtnTitle = "No, thanks";
	private const string kDefaultRateBtnTitle = "Rate it now";
	private const string kDefaultRemindBtnTitle = "Remind me later";

	private const float SECONDS_IN_A_DAY = 86400.0f;
	private const float SECONDS_IN_A_WEEK = 604800.0f;

	private bool _currentChecking;
	private bool _promptShowing;

	private Dictionary<string, object> _localizationDic;
	private Dictionary<string, object> localizationDic {
		get {
			if (_localizationDic == null) {
				TextAsset txt = (TextAsset)Resources.Load("UniRateLocalizationStrings", typeof(TextAsset));
				_localizationDic = UniRateMiniJSON.Json.Deserialize(txt.text) as Dictionary<string, object>;
			}
			return _localizationDic;
		}
	}
	#endregion

	private static UniRate _instance = null;
	/// <summary>
	/// Gets the instance of the UniRate.
	/// If there is not an instance, UniRate will generate a gameObject "UniRateManager" in current scene.
	/// The instance game object will not be destroyed when load new scene.
	/// </summary>
	/// <value>The instance in the scene.</value>
	public static UniRate Instance {
	    get {
	        if (!_instance) {
	            _instance = FindObjectOfType(typeof(UniRate)) as UniRate;

	            if (!_instance) {
					var obj = new GameObject("UniRateManager");
	                _instance = obj.AddComponent<UniRate>();
				} else {
					_instance.gameObject.name = "UniRateManager";
				}

				DontDestroyOnLoad(_instance.gameObject);

				UniRatePlugin.InitUniRateAndroid();
	        }
	        return _instance;
	    }
	}

	/// <summary>
	/// Check the conditions to determine UniRate should show the prompt for rating.
	/// </summary>
	/// <returns><c>true</c>, if should prompt for rating, <c>false</c> otherwise.</returns>
	public bool ShouldPromptForRating() {
		if (previewMode) { //preview mode?
			Debug.Log("UniRate is in preview mode. Make sure you set previewMode to false when release.");
			return true;
		} else if (this.ratedThisVersion) { //is this version rated?
			UniRateDebug.Log("Not prompt. The user has already rated this version");
			return false;
		} else if (!promptAgainForEachNewVersion && this.ratedAnyVersion) { //is already rated?
			UniRateDebug.Log("Not prompt. The user has already rated for some version and promptAgainForEachNewVersion is disabled");
			return false;
		} else if (this.declinedThisVersion) { //is user refused to rate this version?
			UniRateDebug.Log("Not prompt. The user refused to rate this version");
			return false;
		} else if ((this.daysUntilPrompt > 0.0f || this.usesPerWeekForPrompt != 0.0f) && this.firstUsed == DateTime.MaxValue) {	//check for first launch
			UniRateDebug.Log("Not prompt. First launch");
			return false;
		} else if ((DateTime.Now - this.firstUsed).TotalSeconds < this.daysUntilPrompt * SECONDS_IN_A_DAY) {
			UniRateDebug.Log("Not prompt. App was used less than " + this.daysUntilPrompt + " days ago");
			return false;
		} else if (this.usesCount < this.usesUntilPrompt) {	//check use times and event times
			UniRateDebug.Log("Not prompt. App was only used " + this.usesCount + " times");
			return false;
		} else if (this.eventCount < this.eventsUntilPrompt) {
			UniRateDebug.Log("Not prompt. Only " + this.eventCount + " times of events logged");
			return false;
		} else if (this.usesPerWeek < this.usesPerWeekForPrompt) {	//check if usage frequency high enough
			UniRateDebug.Log("Not prompt. Only used " + this.usesPerWeek + " times per week");
			return false;
		} else if (this.lastReminded != DateTime.MaxValue && 
				   (DateTime.Now - this.lastReminded).TotalSeconds < remindPeriod * SECONDS_IN_A_DAY) {
			UniRateDebug.Log("Not prompt. The user askd to be reminded and it is not the time now");
			return false;
		}

		//It should prompt to rate.
		return true;
	}

	/// <summary>
	/// Make a prompts directly. It will configure settings and then open a prompt 
	/// without condition checking if network available. You can use this method to 
	/// implement a rate button in your game. In most case, you will not want to 
	/// prompt users again when they click the "Rate" button, you can use RateIfNetworkAvailable()
	/// instead.
	/// </summary>
	public void PromptIfNetworkAvailable() {
		CheckAndReadyToRate(true);
	}

	/// <summary>
	/// Go to rate page directly. It will configure settings and then switch to the rate
	/// page of your game.  You can use this method to implement a rate button in your game.
	/// </summary>
	public void RateIfNetworkAvailable() {
		CheckAndReadyToRate(false);
	}

	/// <summary>
	/// Shows the prompt, there is no check for conditions or app information getting. 
	/// It depends on <see cref="appStoreID"/>, if you are not set it yourself, do not use this metod (use PromptIfNetworkAvailable instead).
	/// </summary>
	public void ShowPrompt() {
		UniRateDebug.Log("It's time to show prompt");
		if (OnPromptedForRating != null) {
			OnPromptedForRating();
		}

		if (!useCustomizedPromptView) {
			UniRatePlugin.ShowPrompt(this.MessageTitle, 
			                         this.Message, 
			                         this.RateButtonLabel, 
			                         this.CancelButtonLabel, 
			                         this.RemindButtonLabel);
			_promptShowing = true;
            #if UNITY_WP8
            _shouldPoll = true;
            #endif
		}
	}

	/// <summary>
	/// Opens the rate page directly without automatically configuration. Do not call this 
	/// if you did not set the AppID for UniRate yourself. You may want to use RateIfNetworkAvailable()
	/// instead, which can retrive the correct AppiD if the network is available.
	/// </summary>
	public void OpenRatePage() {
		this.ratedThisVersion = true;
		#if UNITY_IOS
		UniRateDebug.Log("Open rating page of URL: " + this.RatingIOSURL);
		Application.OpenURL(this.RatingIOSURL);
		#elif UNITY_ANDROID
		UniRateDebug.Log("Open rating page of URL: " + this.RatingAndroidURL);
		UniRatePlugin.OpenAndroidRatePage(this.RatingAndroidURL);
		#elif UNITY_WP8
		UniRateDebug.Log("Open rating page for WP8");
		UniRatePlugin.OpenWPMarket();
		#endif
	}
	
	/// <summary>
	/// Log a event. The <see cref="eventCount"/> will be increased by calling this method.
	/// </summary>
	/// <param name="withPrompt">If set to <c>true</c>, a prompt will be generated immediately after the event logging.</param>
	public void LogEvent(bool withPrompt) {
		IncreaseEventCount();
		if (withPrompt && ShouldPromptForRating()) {
			PromptIfNetworkAvailable();
		}
	}

	/// <summary>
	/// Reset the UniRate.
	/// </summary>
	public void Reset() {
		PlayerPrefs.DeleteKey(kUniRateRatedVersionKey);
		PlayerPrefs.DeleteKey(kUniRateDeclinedVersionKey);
		PlayerPrefs.DeleteKey(kUniRateLastRemindedKey);
		PlayerPrefs.DeleteKey(kUniRateLastVersionUsedKey);
		PlayerPrefs.DeleteKey(kUniRateFirstUsedKey);
		PlayerPrefs.DeleteKey(kUniRateUseCountKey);
		PlayerPrefs.DeleteKey(kUniRateEventCountKey);

		PlayerPrefs.Save();

		UniRate.Instance.Init();
	}

	void Start() {
		UniRate.Instance.Init();
	}

    #if UNITY_WP8
    private bool _shouldPoll;
    void Update() {
        if (_shouldPoll) {
            UniRateWP.UniRatePromptResult result = UniRateWP.Interface.PollResult();
            switch (result) {
                case UniRateWP.UniRatePromptResult.Rate: {
                    _shouldPoll = false;
                    UniRateUserWantToRate();
                } break;
                case UniRateWP.UniRatePromptResult.Later: {
                    _shouldPoll = false;
                    UniRateUserWantRemind();
                } break;
                case UniRateWP.UniRatePromptResult.Cancel: {
                    _shouldPoll = false;
                    UniRateUserDeclinedPrompt();
                } break;
                default: break;
            }
        }
    }
    #endif

	void Init() {
		//Set app information for UniRate
		if (string.IsNullOrEmpty(appStoreCountry)) {
			appStoreCountry = UniRatePlugin.GetAppStoreCountry();
			UniRateDebug.Log("Get Country Code: " + appStoreCountry);
		}
		if (string.IsNullOrEmpty(applicationVersion)) {
			applicationVersion = UniRatePlugin.GetApplicationVersion();
			UniRateDebug.Log("Get App Version: " + applicationVersion);
		}
		if (string.IsNullOrEmpty(applicationName)) {
			applicationName = UniRatePlugin.GetApplicationName();
			UniRateDebug.Log("Get App Name: " + applicationName);
		}
		if (string.IsNullOrEmpty(applicationBundleID)) {
			applicationBundleID = UniRatePlugin.GetApplicationBundleID();
			UniRateDebug.Log("Get Bundle ID: " + applicationBundleID);
		}

		if (string.IsNullOrEmpty(marketPackageName)) {
			marketPackageName = UniRatePlugin.GetPackageName();
			UniRateDebug.Log("Get Android package name: " + marketPackageName);
		}

		_promptShowing = false;
		
		UniRateLauched();
	}

	void UniRateLauched() {
		if (!IsSameVersion()) {
			//Reset count
			PlayerPrefs.SetString(kUniRateLastVersionUsedKey,applicationVersion);
			UniRatePlayerPrefs.SetDate(kUniRateFirstUsedKey,DateTime.Now);
			PlayerPrefs.SetInt(kUniRateUseCountKey, 0);
			PlayerPrefs.SetInt(kUniRateEventCountKey, 0);
			PlayerPrefs.DeleteKey(kUniRateLastRemindedKey);

			PlayerPrefs.Save();

			if (OnDetectAppUpdated != null) {
				OnDetectAppUpdated();
			}
		}

		IncreaseUseCount();
		if (promptAtLaunch && ShouldPromptForRating()) {
			PromptIfNetworkAvailable();
		}
	}

	bool IsSameVersion() {
		if (!string.IsNullOrEmpty(applicationVersion)) {
			return string.Equals(PlayerPrefs.GetString(kUniRateLastVersionUsedKey),applicationVersion);
		}
		return false;
	}

	void OnApplicationPause(bool pauseStatus) {
        if (!pauseStatus && _instance != null) {
			IncreaseUseCount();
			if (promptAtLaunch && !_promptShowing && ShouldPromptForRating()) {
				PromptIfNetworkAvailable();
			}
        }
    }

	void IncreaseUseCount() {
		this.usesCount ++;
	}

	void IncreaseEventCount() {
    	this.eventCount ++;
	}

	void CheckAndReadyToRate(bool showPrompt) {
		if (!_currentChecking) {
			#if UNITY_IOS
			_currentChecking = true;
			StartCoroutine(CheckForConnectivityInBackground(showPrompt));
			#elif UNITY_ANDROID || UNITY_WP8
			if (showPrompt) {
				ReadyToPrompt();
			} else {
				OpenRatePage();
			}
			#endif
		}
	}

	//This check is only for iOS
	IEnumerator CheckForConnectivityInBackground(bool showPrompt) {
		string iTunesServiceURL= "";
		if (!string.IsNullOrEmpty(appStoreCountry)) {
			iTunesServiceURL = string.Format(kUniRateAppLookupURLFormat, appStoreCountry + "/");
		} else {
			iTunesServiceURL = string.Format(kUniRateAppLookupURLFormat, "");
		}
		 
		if (appStoreID != 0) {
			iTunesServiceURL = iTunesServiceURL + "?id=" + appStoreID;
		} else {
			iTunesServiceURL = iTunesServiceURL + "?bundleId=" + applicationBundleID;
		}
		UniRateDebug.Log("Checking app info: " + iTunesServiceURL);

		bool errorHappened = false;

		WWW www = new WWW(iTunesServiceURL);
		yield return www;
		if (String.IsNullOrEmpty(www.error)) { //No error. Try parse app info from the returned text
			UniRateAppInfo info = new UniRateAppInfo(www.text);
			if (info.validAppInfo) {
				if (info.bundleId.Equals(this.applicationBundleID)) {	//Everything goes ok
					if (appStoreID == 0) {
						appStoreID = info.appID;
						UniRateDebug.Log("UniRate found the app, app id: " + appStoreID);
					}
					if (this.onlyPromptIfLatestVersion && !this.previewMode) {
						if (!applicationVersion.Equals(info.version)) {
							UniRateDebug.Log("No prompt because it is not the version in app store and you set onlyPromptIfLatestVersion.");
							errorHappened = true;
							UniRateFailWithError(Error.NotTheLatestVersion);
						}
					}
				} else {
					Debug.LogWarning("The bundle Id is not the same. Appstore: " + info.bundleId + " vs AppSetting:" + applicationBundleID);
					errorHappened = true;
					UniRateFailWithError(Error.BundleIdDoesNotMatchAppStore);
				}
			} else {
				if (appStoreID != 0) { //We have a specified app id
					Debug.LogWarning("No App info found with this app Id " + appStoreID);
					errorHappened = true;
					UniRateFailWithError(Error.AppNotFoundOnAppStore);
				} else {
					Debug.Log("No App info found with this bundle Id " + applicationBundleID);
					Debug.Log("Could not find your app on AppStore. It is normal when your app is not released, don't worry about this message.");
				}
			}
		} else {
			UniRateDebug.Log("Error happend in loading app information. Maybe due to no internet connection.");
			errorHappened = true;
			UniRateFailWithError(Error.NetworkError);
		}

		//Every thing goes ok. Prompt to rating now.
		if (!errorHappened) {
			_currentChecking = false;
			if (showPrompt) {
				ReadyToPrompt();
			} else {
				OpenRatePage();
			}
		}
	}

	void ReadyToPrompt() {
		//Ask the delegate if we can prompt, if there is a delegate.
		if (ShouldUniRatePromptForRating != null && !ShouldUniRatePromptForRating()) {
			UniRateDebug.Log("Not display prompt because ShouldUniRatePromptForRating returns false.");
			return;
		}

		ShowPrompt();
	}

	void UniRateFailWithError(Error error) {
		if (OnUniRateFaild != null) {
			OnUniRateFaild(error);
		}
	}

	void UniRateUserDeclinedPrompt() {
		UniRateDebug.Log("User declined the prompt");
		_promptShowing = false;
		this.declinedThisVersion = true;

		if (OnUserDeclinedToRate != null) {
			OnUserDeclinedToRate();
		}
	}

	void UniRateUserWantRemind() {
		UniRateDebug.Log("User wants to be reminded later");
		_promptShowing = false;
		this.lastReminded = DateTime.Now;

		if (OnUserWantReminderToRate != null) {
			OnUserWantReminderToRate();
		}
	}
	
	void UniRateUserWantToRate() {
		UniRateDebug.Log("User wants to rate");
		_promptShowing = false;

		if (OnUserAttemptToRate != null) {
			OnUserAttemptToRate();
		}

		if (ShouldUniRateOpenRatePage != null && !ShouldUniRateOpenRatePage()) {
			UniRateDebug.Log("Not open rate page because ShouldUniRateOpenRatePage() returning false");
		} else {
			OpenRatePage();
		}
	}

	float iOSVersion {
		get {
		    // SystemInfo.operatingSystem returns something like iPhone OS 6.1
		    float osVersion = -1f;
		    string versionString = SystemInfo.operatingSystem.Replace("iPhone OS ", "");
		    float.TryParse(versionString.Substring(0, 1), out osVersion);
		    return osVersion;
		}
	}

	string GetLocalizedMessageTitle() {
		string formatString = kDefaultTitle;
		if (autoLocalization) {
			string language = Application.systemLanguage.ToString();
			formatString = GetContentForLanguageAndKey(language, "title") ?? formatString;
		}
		return string.Format(formatString,applicationName);
	}

	string GetLocalizedMessage() {
		string formatString = kDefaultMessage;
		if (autoLocalization) {
			string language = Application.systemLanguage.ToString();
			formatString = GetContentForLanguageAndKey(language, "body") ?? formatString;
		}
		return string.Format(formatString,applicationName);
	}

	string GetLocalizedCancelButtonLabel() {
		string labelString = kDefaultCancelBtnTitle;
		if (autoLocalization) {
			string language = Application.systemLanguage.ToString();
			labelString = GetContentForLanguageAndKey(language, "cancel") ?? labelString;
		}
		return labelString;
	}

	string GetLocalizedRemindButtonLabel() {
		string labelString = kDefaultRemindBtnTitle;
		if (autoLocalization) {
			string language = Application.systemLanguage.ToString();
			labelString = GetContentForLanguageAndKey(language, "remind") ?? labelString;
		}
		return labelString;
	}

	string GetLocalizedRateButtonLabel() {
		string labelString = kDefaultRateBtnTitle;
		if (autoLocalization) {
			string language = Application.systemLanguage.ToString();
			labelString = GetContentForLanguageAndKey(language, "rate") ?? labelString;
		}
		return labelString;
	}

	string GetContentForLanguageAndKey(string lang, string key) {
		if (this.localizationDic != null && this.localizationDic.ContainsKey(lang)) {
			Dictionary<string, object> langDetail = this.localizationDic[lang] as Dictionary<string, object>;
			if (langDetail != null && langDetail.ContainsKey(key)) {
				return langDetail[key] as string;
			}
		}
		return null;
	}
}

/// <summary>
/// Debug helper class for UniRate. Just make it easier for debug.
/// </summary>
public class UniRateDebug {
	public static void Log(object message) {
		#if UniRateDebug
		Debug.Log("UniRateDebug: " + message);
		#endif

        #if UniRateDebug && UNITY_WP8
        if (Application.platform == RuntimePlatform.WP8Player) {
              UniRateWP.Interface.Log("UniRateDebug: " + message);  
        }
        #endif
    }
}

/// <summary>
/// A powerful version of PlayerPrefs. Can save and load DateTime
/// </summary>
public class UniRatePlayerPrefs {
	/// <summary>
	/// Sets the date in PlayerPrefs.
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	public static void SetDate(string key, DateTime value) {
		long ticks = value.Ticks;
		PlayerPrefs.SetString(key, ticks.ToString());
	}

	/// <summary>
	/// Gets the date from PlayerPrefs.
	/// </summary>
	/// <returns>The date.</returns>
	/// <param name="key">Key.</param>
	public static DateTime GetDate(string key) {
		string str = PlayerPrefs.GetString(key);
		long result = 0;
		if (Int64.TryParse(str,out result)) {
			return new DateTime(result);
		} else {
			return DateTime.MaxValue;
		}
	}
}

/// <summary>
/// The market type for Android store. Difference url scheme will be used for different store.
/// </summary>
public enum UniRateMarketType {
	GooglePlay = 0,
	AmazonAppstore = 1,
}
