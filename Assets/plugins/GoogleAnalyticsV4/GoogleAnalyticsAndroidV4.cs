/*
  Copyright 2014 Google Inc. All rights reserved.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
  GoogleAnalyticsAndroidV4 handles building hits using the Android SDK.
  Developers should call the methods in GoogleAnalyticsV4, which will call the
  appropriate methods in this class if the application is built for Android.
*/
public class GoogleAnalyticsAndroidV4 : IDisposable {
#if UNITY_ANDROID && !UNITY_EDITOR
  private string trackingCode;
  private string appVersion;
  private string appName;
  private string bundleIdentifier;
  private int dispatchPeriod;
  private int sampleFrequency;
  //private GoogleAnalyticsV4.DebugMode logLevel;
  private bool anonymizeIP;
  private bool adIdCollection;
  private bool dryRun;
  private int sessionTimeout;
  private AndroidJavaObject tracker;
  private AndroidJavaObject logger;
  private AndroidJavaObject currentActivityObject;
  private AndroidJavaObject googleAnalyticsSingleton;
  //private bool startSessionOnNextHit = false;
  //private bool endSessionOnNextHit = false;

  internal void InitializeTracker() {
    Debug.Log("Initializing Google Analytics Android Tracker.");

    using (AndroidJavaObject googleAnalyticsClass = new AndroidJavaClass("com.google.android.gms.analytics.GoogleAnalytics"))
    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
      currentActivityObject = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
      googleAnalyticsSingleton = googleAnalyticsClass.CallStatic<AndroidJavaObject>("getInstance", currentActivityObject);

      tracker = googleAnalyticsSingleton.Call<AndroidJavaObject>("newTracker", trackingCode);

      googleAnalyticsSingleton.Call("setLocalDispatchPeriod", dispatchPeriod);
      googleAnalyticsSingleton.Call("setDryRun", dryRun);

      tracker.Call("setSampleRate", (double)sampleFrequency);
      tracker.Call("setAppName", appName);
      tracker.Call("setAppId", bundleIdentifier);
      tracker.Call("setAppVersion", appVersion);
      tracker.Call("setAnonymizeIp", anonymizeIP);
      tracker.Call("enableAdvertisingIdCollection", adIdCollection);
    }
  }

  internal void SetTrackerVal(Field fieldName, object value) {
    object[] args = new object[] { fieldName.ToString(), value };
    tracker.Call("set", args);
  }

  private void SetSessionOnBuilder(AndroidJavaObject hitBuilder) {
  }

  internal void StartSession() {
    //startSessionOnNextHit = true;
  }

  internal void StopSession() {
    //endSessionOnNextHit = true;
  }

  public void SetOptOut(bool optOut) {
    googleAnalyticsSingleton.Call("setAppOptOut", optOut);
  }

  internal void LogScreen (AppViewHitBuilder builder) {
    tracker.Call("setScreenName", builder.GetScreenName());

    AndroidJavaObject eventBuilder = new AndroidJavaObject("com.google.android.gms.analytics.HitBuilders$ScreenViewBuilder");
    object[] builtScreenView = new object[] { eventBuilder.Call<AndroidJavaObject>("build") };
    tracker.Call("send", builtScreenView);
  }

  internal void LogEvent(EventHitBuilder builder) {
    AndroidJavaObject eventBuilder = new AndroidJavaObject("com.google.android.gms.analytics.HitBuilders$EventBuilder");
    eventBuilder.Call<AndroidJavaObject>("setCategory", new object[] { builder.GetEventCategory() });
    eventBuilder.Call<AndroidJavaObject>("setAction", new object[] { builder.GetEventAction() });
    eventBuilder.Call<AndroidJavaObject>("setLabel", new object[] { builder.GetEventLabel() });
    eventBuilder.Call<AndroidJavaObject>("setValue", new object[] { builder.GetEventValue() });
    object[] builtEvent = new object[] { eventBuilder.Call<AndroidJavaObject>("build") };
    tracker.Call("send", builtEvent);
  }

  internal void LogTransaction(TransactionHitBuilder builder) {
  }

  internal void LogItem(ItemHitBuilder builder) {
  }

  public void LogException(ExceptionHitBuilder builder) {
  }

  public void LogSocial(SocialHitBuilder builder) {
  }

  public void LogTiming(TimingHitBuilder builder) {
  }

  public void DispatchHits() {
  }

  public void SetSampleFrequency(int sampleFrequency) {
    this.sampleFrequency = sampleFrequency;
  }

  public void ClearUserIDOverride() {
    SetTrackerVal(Fields.USER_ID, null);
  }

  public void SetTrackingCode(string trackingCode) {
    this.trackingCode = trackingCode;
  }

  public void SetAppName(string appName) {
    this.appName = appName;
  }

  public void SetBundleIdentifier(string bundleIdentifier) {
    this.bundleIdentifier = bundleIdentifier;
  }

  public void SetAppVersion(string appVersion) {
    this.appVersion = appVersion;
  }

  public void SetDispatchPeriod(int dispatchPeriod) {
    this.dispatchPeriod = dispatchPeriod;
  }

  public void SetLogLevelValue(GoogleAnalyticsV4.DebugMode logLevel) {
    //this.logLevel = logLevel;
  }

  public void SetAnonymizeIP(bool anonymizeIP) {
    this.anonymizeIP = anonymizeIP;
  }

  public void SetAdIdCollection(bool adIdCollection) {
    this.adIdCollection = adIdCollection;
  }

  public void SetDryRun(bool dryRun) {
    this.dryRun = dryRun;
  }

#endif
  public void Dispose() {
#if UNITY_ANDROID && !UNITY_EDITOR
    googleAnalyticsSingleton.Dispose();
    tracker.Dispose();
#endif
  }
}
