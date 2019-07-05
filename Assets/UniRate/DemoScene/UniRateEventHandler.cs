//
//	UniRateEventHandler.cs
//  Created by Wang Wei(@onevcat) on 2013-7-15.
//

using UnityEngine;
using System.Collections;

public class UniRateEventHandler : MonoBehaviour {
	//By default, you need not to implement these event method. It is just a demo to show what you can do.
	//Only when necessary, you should add methods for the events and handle them.
	void Awake() {
		UniRate.Instance.ShouldUniRatePromptForRating += ShouldUniRatePromptForRating;
		UniRate.Instance.ShouldUniRateOpenRatePage += ShouldUniRateOpenRatePage;
		UniRate.Instance.OnPromptedForRating += OnPromptedForRating;
		UniRate.Instance.OnDetectAppUpdated +=  OnDetectAppUpdated;
		UniRate.Instance.OnUniRateFaild += OnUniRateFaild;
		UniRate.Instance.OnUserAttemptToRate += OnUserAttemptToRate;
		UniRate.Instance.OnUserDeclinedToRate += OnUserDeclinedToRate;
		UniRate.Instance.OnUserWantReminderToRate += OnUserWantReminderToRate;
	}

	//UniRate asks if it should pop a prompt when it wants to show a prompt.
	//If not implemented, means true.
	bool ShouldUniRatePromptForRating() {
		//For example, if the user is in game progress, it is better not to interrupt for prompt.
		//if (inGame) {
		//	return false;
		//}
		return true;
	}

	//UniRate asks if it should open the rate page when user clicked the Rate button.
	//If not implemented, means true.
	bool ShouldUniRateOpenRatePage() {
		return true;
	}

	//UniRate will raise this event before it show a prompt alert.
	//If you want to use your customized prompt window, you should set UniRate's useCustomizedPromptView to true in scene.
	//Then you can pop up a customized window in this metod.
	void OnPromptedForRating() {
		//aBeautifalPromptAlert.Show();

		//When user tapped "Rate" in your alert, you should send a "UniRateUserWantToRate" message to UniRate
		//UniRate.Instance.SendMessage("UniRateUserWantToRate");

		//When user tapped "Cancel" in your alert, you should send a "UniRateUserDeclinedPrompt" message to UniRate
		//UniRate.Instance.SendMessage("UniRateUserDeclinedPrompt");

		//When user tapped "Cancel" in your alert, you should send a "UniRateUserWantRemind" message to UniRate
		//UniRate.Instance.SendMessage("UniRateUserWantRemind");
	}

	//Raised when user just intalled a new version.
	void OnDetectAppUpdated() {
		Debug.Log("A new version is installed. Current version: " + UniRate.Instance.applicationVersion);
	}

	//There is an error during getting app info
	void OnUniRateFaild(UniRate.Error error) {
		Debug.Log(error);
	}

	//Raised when user tapped the "Rate" button
	void OnUserAttemptToRate() {
		Debug.Log("Yeh, great, user want to rate us!");
	}

	//Raised when user tapped the "Cancel" button
	void OnUserDeclinedToRate() {
		Debug.Log("User declined the rate prompt.");
	}

	//Raised when user tapped the "Remind" button
	void OnUserWantReminderToRate() {
		Debug.Log("User wants to be reminded later.");
	}

	void OnDestroy() {
		UniRate.Instance.ShouldUniRatePromptForRating -= ShouldUniRatePromptForRating;
		UniRate.Instance.ShouldUniRateOpenRatePage -= ShouldUniRateOpenRatePage;
		UniRate.Instance.OnPromptedForRating -= OnPromptedForRating;
		UniRate.Instance.OnDetectAppUpdated -=  OnDetectAppUpdated;
		UniRate.Instance.OnUniRateFaild -= OnUniRateFaild;
		UniRate.Instance.OnUserAttemptToRate -= OnUserAttemptToRate;
		UniRate.Instance.OnUserDeclinedToRate -= OnUserDeclinedToRate;
		UniRate.Instance.OnUserWantReminderToRate -= OnUserWantReminderToRate;
	}
}
