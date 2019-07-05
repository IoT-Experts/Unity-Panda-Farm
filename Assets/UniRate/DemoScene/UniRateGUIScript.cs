//
//	UniRateGUIScript.cs
//  Created by Wang Wei(@onevcat) on 2013-7-15.
//
using UnityEngine;
using System.Collections;
using System.Text;

public class UniRateGUIScript : MonoBehaviour {
	//App basic info.
	private string bundleIDString = "Bundle ID: {0}\n";
	private string appIDString = "App ID: {0}\n";
	private string appNameString = "App Name: {0}\n";
	private string appVersionString = "App Version: {0}\n";

	//Used count info.
	private string usesUntilPromptString = "Uses Until Prompt: {0}\n";
	private string usedString = "Already Used Count: {0}\n";

	//Event info.
	private string eventsUntilPromptString = "Events Until Prompt: {0}\n";
	private string eventsHappenedString = "Events Happened: {0}\n";

	//Day use info.
	private string daysUntilPromptString = "Days Until Prompt:{0}\n";
	private string daysUsedString = "Days From First Use:{0}\n";

	//Use frequency info.
	private string usesPerWeekToPromptString = "Uses per week to prompt:{0}\n";
	private string usesPerWeekString = "Current uses per week:{0}\n";

	private string remindString = "Remind after:{0} days\n";

	void OnGUI() {
 		StringBuilder sb = new StringBuilder();

        sb.Append(string.Format(bundleIDString, UniRate.Instance.applicationBundleID));
        sb.Append(string.Format(appIDString, UniRate.Instance.appStoreID));
        sb.Append(string.Format(appNameString, UniRate.Instance.applicationName));
        sb.Append(string.Format(appVersionString, UniRate.Instance.applicationVersion));

        sb.Append(string.Format(usesUntilPromptString, UniRate.Instance.usesUntilPrompt));
        sb.Append(string.Format(usedString, UniRate.Instance.usesCount));

        sb.Append(string.Format(eventsUntilPromptString, UniRate.Instance.eventsUntilPrompt));
        sb.Append(string.Format(eventsHappenedString, UniRate.Instance.eventCount));

        sb.Append(string.Format(daysUntilPromptString, UniRate.Instance.daysUntilPrompt));
        sb.Append(string.Format(daysUsedString, UniRate.Instance.usedDays));

        sb.Append(string.Format(usesPerWeekToPromptString, UniRate.Instance.usesPerWeekForPrompt));
        sb.Append(string.Format(usesPerWeekString, UniRate.Instance.usesPerWeek));

		if (UniRate.Instance.waitingByRemindLater) {
            sb.Append(string.Format(remindString, UniRate.Instance.leftRemindDays));
		}

		GUI.Label(new Rect(0,0,300,500),sb.ToString());
        
		if (GUI.Button(new Rect(0,Screen.height - 50 ,100,50),"Rate")) {
			UniRate.Instance.RateIfNetworkAvailable();
		}

		if (GUI.Button(new Rect(100,Screen.height - 50 ,100,50),"Rate Prompt")) {
			UniRate.Instance.PromptIfNetworkAvailable();
		}

		if (GUI.Button(new Rect(200,Screen.height - 50,100,50),"LogEvent")) {
			UniRate.Instance.LogEvent(true);
		}

		if (GUI.Button(new Rect(300,Screen.height - 50,100,50),"Reset")) {
			UniRate.Instance.Reset();
		}
	}
}
