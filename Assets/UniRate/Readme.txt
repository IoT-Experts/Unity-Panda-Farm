## UniRate - A better drop-in solution for rating mobile apps 

Version 1.2.1

![UniRate](https://github.com/onevcat/UniRate/blob/master/img/unirate0.png?raw=true)

## What is UniRate

**UniRate** is a Unity package to help you promote you mobile games (iOS, Android and Windows Phone 8) by prompting users to rate the app after using it for a few days or times. Integrating `UniRate` to your project is deadly simple as a drag & drop, but it is powerful and configurable for some advanced usage. It will count the using of the times and days automatically and if the conditions you set are met, a native prompt will pop up and user can decide whether rate it or not. 

This approach is one of the best ways to get positive app reviews by targeting only regular users (who presumably like the game or they wouldn't keep playing it).

You can purchase `UniRate` from [this link of Unity Asset Store](https://www.assetstore.unity3d.com/#/content/10116) and use it in all your projects with the purchase Unity ID.

## Getting Start

It is very easy to integrate and use the basic feature of `UniRate` in your project.

1. Download and import the Unity package of `UniRate` into your project.
2. Find and drag the prefab named `UniRateManager` which located in `UniRate` folder, to the first scene of your project.
3. And there is no step 3.

![Drag & Drop, no step 3](https://github.com/onevcat/UniRate/blob/master/img/unirate1.png?raw=true)


`UniRate` typically requires no configuration at all and will simply run automatically, using the information of application(bundle ID in iOS, package name in Android and App name in WP8) and look app ID and versions up on Internet.

## Configuration

### Prompt Condition

By default, `UniRate` will prompt your users when they opened your app **more than 10 times** AND installed **at lease 3 days ago**. You can change these settings as you like. `UniRate` provides an easy way to configure them in Unity Editor's Inspector Panel. You can tweak `UniRate` and change these prompt conditions to satisfy your demand.

![Tweak as you like in Unity Editor](https://github.com/onevcat/UniRate/blob/master/img/unirate2.png?raw=true)

The properties you may want to modify are listed below:

#### Uses Until Prompt

This is the minimum number of times the user must launch the app before they are prompted to rate it. This avoids the scenario where a user runs the app once, doesn't look at it for weeks and then launches it again, only to be immediately prompted to rate it. The minimum use count ensures that only frequent users are prompted. The prompt will appear only after the specified number of days AND uses has been reached. This defaults to 10 uses.

#### Events Until Prompt

For some apps, launches are not a good metric for usage. For example the user can't write an informed review until they've reached a particular level or finished the tutorial. In this case you can manually log significant events and have the prompt appear after a predetermined number of these events by calling `UniRate.Instance.LogEvent(true)` from anywhere. Like the `Uses Until Prompt` setting, the prompt will appear only after the specified number of days AND events, however once the day threshold is reached, the prompt will appear if BOTH the event threshold AND uses threshold is reached. This defaults to 0 events, which meas there is no require for an event.

#### Days Until Prompt

This is the number of days the user must have had the app installed before they are prompted to rate it. The time is measured from the first time THIS version of app is launched. This is a floating point value, so it can be used to specify a fractional number of days (e.g. 0.5). The default value is 3 days.

#### Uses Per Week For Prompt

If you are less concerned with the total number of times the app is used, but would prefer to use the frequency of times the app is used, you can use the usesPerWeekForPrompt property to set a minimum threshold for the number of times the user must launch the app per week (on average) for the prompt to be shown. Note that this is the average since the app was installed, so if the user goes for a long period without running the app, it may throw off the average. The default value is 0.

#### Remind Period

How long the app should wait before reminding a user to rate after they select the "remind me later" option. A value of zero means the app will remind the user next launch. The default value is 1 day.

### Prompt Pop up

By default, `UniRate` will prompt users by pop up a system dialog window with three buttons (UIAlertView in iOS, AlertDialog in Android and a custom pop dialog in WP8). Users can choose to "Rate", "Remind" or "Cancel". You can customize the prompt title and message in the Inspector Panel. There are also customizable button text for you.

#### Message Title
The title displayed for the rating prompt. If this value is not set or is an empty string, a default value will be used for display. Default message title is "Rate {applicationName}".

#### Message
The message body you set and be displayed in prompt pop-up. If this value is not set or is an empty string, a default value will be used for display. Default value is "If you enjoy {applicationName}, would you mind taking a moment to rate it? It will not take more than a minute. Thanks for your support!".

#### Cancel Button Label
The button label for the button to dismiss the rating prompt without rating the app. If this value is not set or is an empty string, a default value will be used for display. Default value is "No, Thanks".

#### Remind Button Label
The button label for the button the user presses if they don't want to rate the app immediately, but do want to be reminded about it in future. If this value is not set or is an empty string, a default value will be used for display. Default value is "Remind Me Later".

#### Rate Button Label
The button label for the button the user presses if they do want to rate the app. If this value is not set or is an empty string, a default value will be used for display. Default value is "Rate It Now".

## Events and Delegate

`UniRate` has a set of delegate methods and will raise the corresponding event at proper time. You can control the action of `UniRate` by implementing methods in your own code to handle these delegates and events.

### UniRate should do something or not

#### ShouldUniRatePromptForRatingDelegate

`UniRate` asks if it should pop a prompt when it wants to show a prompt, before it is going to do so. By default, `UniRate` will check the conditions and pop up a prompt if all conditions are met. But it is not a good idea to prompt your users in some situation. For example, it make no sense to show a prompt when they are in your game progress. When it is not a proper time to pop up prompt, you should return false here and remember a shouldPrompt flag, then call `UniRate.Instance.PromptIfNetworkAvailable()` later. If it is not implemented, UniRate will always show prompt.

#### ShouldUniRateOpenRatePage

`UniRate` asks if it should open the rate page when user clicked the Rate button. Return false if you want to implement your own ratings page display logic. Else the AppStore, Android Market page or Windows store page of your app will be opened. If it is not implemented, UniRate will always open the rate page.

### UniRate did raised some events

#### OnPromptedForRating

This method is called immediately before the rating prompt is displayed. This is useful if you use analytics to track what percentage of users see the prompt and then go to rate for your app. And it is also a change for you to use your customized view intead of system alert view/dialog. See Advanced Usage's Customize Prompt section for more about showing customized prompt.

#### OnDetectAppUpdated

This method is called when a new version of the app is installed and opened for the first time. All count (used count, days count, etc) was reseted.

#### OnUniRateFaild

This method is called when there is an error during getting app info from Internet. A UniRate.Error will be sent with this method. The error could be one of these: `BundleIdDoesNotMatchAppStore`, `AppNotFoundOnAppStore`, `NotTheLatestVersion`, `NetworkError`. You can choose to show a UI to tell your users what gets wrong, you can also choose to let it fail silently. How to treat these errors depends on you.

#### OnUserAttemptToRate

This method is called when the user pressed the rate button in the rating prompt.

#### OnUserDeclinedToRate

This method is called when the user pressed the cancel button in the rating prompt.

#### OnUserWantReminderToRate

This method is called when the user pressed the remind button and asked to be reminded to rate the app in the rating prompt.

## Advanced Usage

The basic usage of `UniRate` is clear and easy. You can also try some advanced usage of `UniRate` if you want. I list some frequently usage below.

### Customize Prompt

Sometimes you may not want the system alert view/dialog for prompting because you prefer a better look for your game. You can use a customized prompt view (a well-designed UI produced by your designer for example) instead.

First, you need to switch the `Use Customized Prompt View` to true in the Inspector Panel of your Unity Editor. It will prevent `UniRate` to pop up a system prompt view. Then you can implement a listener to the `OnPromptedForRating` event and write your code to pop up your own prompt view there. You can choose any tool you like for showing the prompt UI (Unity's GUI, NGUI or anything else). When the user interact your customised UI, you can send the corresponding method to `UniRate` to handle the input result. In detail, you could call `UniRate.Instance.SendMessage("UniRateUserWantToRate");`, `UniRate.Instance.SendMessage("UniRateUserDeclinedPrompt");` or `UniRate.Instance.SendMessage("UniRateUserWantRemind");`. When `UniRate` receives these message, it will continue the standard flow and open rate page or setup a remind for you.

### Setting app id/package name and Rating URL

By default, `UniRate` dose not require any setting of app id and rating url. It can get all needed information from app's bundle and AppStore. If you require a different rating url beside AppStore or Google Play market, you can specify it in the `Rating iOS URL` and `Rating Android URL`. It is also useful if you want to log some user interaction with `UniRate`. There is no url customization for WP8, because UniRate will use the review task of WP8 SDK, so there is no chance to open a customized url.

There is a setting for App ID (iOS) and package name (Android), but in most cases, you have not to set it yourself. Only when you have your own reasons to lead your users to a different page or you are sure about your app id, you will set it yourself. `UniRate` will use the App ID and package name to determine the rating url (if the rating url is not set).

There is also a property called `Market Type`, which is only used for rating in Android devices. You may distribute your game in Google Play market or Amazon App Store. You can choose which market you want lead your users to and UniRate will specify the correct url for you. If you release your game both in two market, please remember to change the value when you export your apk file.

### Implement a manually review btutton

Although `UniRate` will prompt automatically, you may also want a "Rate me" button in your UI and users can use that button to rate your app if they can not wait to say great to you. If you just open a url when tapping the button, `UniRate` will not know user is already rated, so it might prompt user to rate again. It is not so good. So if you want to make your own "Rate me" button, you'd better to call `UniRate` method to pop up prompt and rating page. 

In most case, you may want to call `UniRate.Instance.RateIfNetworkAvailable()` to make UniRate check the APP ID automatically and then navigate your users to the correct rating page. If you have already set the App ID for iOS, you can also call `UniRate.Instance.OpenRatePage()` directly to open the rating page. Sometimes, if you want to prompt users again for confirming if they want to rate or just a mis-touch with the rate button, you can use `UniRate.Instance.PromptIfNetworkAvailable()` to get app's information and show a prompt and let user to rate. You can also use `UniRate.Instance.ShowPrompt()` to prompt without an Internet check. Be caution, if you do not set the AppID, DO NOT use OpenRatePage or ShowPrompt directly.

### Localization

`UniRate` is localized for quite a lot of languages. You can find the localization file under the path in the package: `/UniRate/Resources/UniRateLocalizationStrings.txt`. UniRate is using Unity's Application.systemLanguage for localization. If you want to add your own localization which is not existing yet, just follow the format in the localization file and add your own key.

### Debug Mode

If you tick the `Preview Mode` on, `UniRate` will prompt every time it checks the conditions. It is useful when you are developing the appearance of prompt. Please remember to turn it off when you release your game, or maybe you will get thousands of 1 star. :(

By default, UniRate will not log the most important events. If you are wondering what is happened and eager more control, you can turn on the debug logs by adding a `UniRateDebug` symbol in Scripting Define Symbols setion in Player Settings in Unity. It is recommanded to remove it when your game release.

![Add UniRateDebug to Scripting Define Symbols](https://github.com/onevcat/UniRate/blob/master/img/unirate3.png?raw=true)

## Demo Example

There is a demo file in the DemoScene folder, in which shows the basic usage of `UniRate`. `UniRateGUIScript` shows some information `UniRate` got, including all conditions. You can also feel free to click the button to see how to use public methods. `UniRateEventHandler` implements all events, which would be a guide for advanced use of `UniRate`.

## Script Reference & Support Forum

You can find the [script reference here](http://unirate.onevcat.com/reference). There is also a [support forum](https://groups.google.com/forum/#!forum/unirate) for you to ask anything about `UniRate`. You can also [submit an issue](https://github.com/onevcat/UniRate/issues) if you are sure something goes wrong. Once confirmed, I will fix them as soon as possible. Hope `UniRate` can help you and you can use it happily!
