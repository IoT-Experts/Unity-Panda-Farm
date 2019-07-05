//
//	UniRateAppInfo.cs
//  Created by Wang Wei(@onevcat) on 2013-7-15.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UniRateAppInfo {
	public bool validAppInfo;

	public string bundleId;
	public int appStoreGenreID;
	public int appID;
	public string version;

	private const string kAppInfoResultsKey = "results";
	private const string kAppInfoBundleIdKey = "bundleId";
	private const string kAppInfoGenreIdKey = "primaryGenreId";
	private const string kAppInfoAppIdKey = "trackId";
	private const string kAppInfoVersion = "version";


	public UniRateAppInfo(string jsonResponse) {
		Dictionary<string, object> response = UniRateMiniJSON.Json.Deserialize(jsonResponse) as Dictionary<string, object>;
		if (response != null) {
			List<object> results = response[kAppInfoResultsKey] as List<object>;
			if (results != null && results.Count > 0) {
				Dictionary<string, object> result = results[0] as Dictionary<string, object>;
				if (result != null) {
					bundleId = result[kAppInfoBundleIdKey] as string;
					appStoreGenreID = Convert.ToInt32(result[kAppInfoGenreIdKey]);
					appID = Convert.ToInt32(result[kAppInfoAppIdKey]);
					version = result[kAppInfoVersion] as string;
					validAppInfo = true;
				}
			}
		}
	}
}
