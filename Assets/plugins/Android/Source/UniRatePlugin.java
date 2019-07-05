/*
 * Copyright (C) 2013 onevcat
 * Copyright (C) 2013 OneV's Den
 */

package com.onevcat.unirate;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import android.app.Activity;
import android.os.Bundle;
import android.content.res.Resources;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.net.Uri;
import android.content.Intent;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;

class UniRatePluginInterface
{
	private String mGameObject;

	public UniRatePluginInterface(final String gameObject)
	{
		mGameObject = gameObject;
	}

	public void call(String message)
	{
		UnityPlayer.UnitySendMessage(mGameObject, "CallFromJS", message);
	}
}

public class UniRatePlugin
{
	private Resources res = null;
	private String name = null;

	public String GetPackageName() {
		final Activity a = UnityPlayer.currentActivity;
		String s = a.getPackageName();

		return s;
	}

	public String GetAppName() {
		final Activity a = UnityPlayer.currentActivity;
		int stringID = a.getApplicationInfo().labelRes;
		String appName = a.getString(stringID);

		return appName;
	}

	public String GetAppVersion() {
		final Activity a = UnityPlayer.currentActivity;
		String version;
		try {
			PackageManager manager = a.getPackageManager();
			PackageInfo info = manager.getPackageInfo(a.getPackageName(), 0);
			version = info.versionName;
		} catch (NameNotFoundException e) {
			version = "?";
		}
		// PackageInfo pInfo = a.getPackageManager().getPackageInfo(GetPackageName(), 0);
		// String version = pInfo.versionName;
		Log.d("UniRate",version);
		return version;
	}

	public void OpenRatePage(String url) {
		final Activity a = UnityPlayer.currentActivity;

		Uri uri = Uri.parse(url);
		Intent goToMarket = new Intent(Intent.ACTION_VIEW, uri);
  		a.startActivity(goToMarket);
	}

	public void ShowPrompt(	String aTitle, 
							String aMsg, 
							String aRateTitle, 
							String aCanceltitle, 
							String aRemindTitle) 
	{
		final Activity a = UnityPlayer.currentActivity;
		final String title = aTitle;
		final String msg = aMsg;
		final String rateTitle = aRateTitle;
		final String canceltitle = aCanceltitle;
		final String remindTitle = aRemindTitle;

		a.runOnUiThread(new Runnable() {public void run() {
			DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {
	    		@Override
	    		public void onClick(DialogInterface dialog, int which) {
	        		switch (which){
	        			case DialogInterface.BUTTON_POSITIVE: 
	        				UnityPlayer.UnitySendMessage("UniRateManager", "UniRateUserWantToRate", "");
	            		break;

	        			case DialogInterface.BUTTON_NEGATIVE:
	            			UnityPlayer.UnitySendMessage("UniRateManager", "UniRateUserDeclinedPrompt", "");
	            		break;

	            		case DialogInterface.BUTTON_NEUTRAL:
	            			UnityPlayer.UnitySendMessage("UniRateManager", "UniRateUserWantRemind", "");
	            		break;

	        		}
	    		}
			};
			AlertDialog.Builder builder = new AlertDialog.Builder(UnityPlayer.currentActivity);
			builder.setTitle(title).setMessage(msg).setPositiveButton(rateTitle, dialogClickListener).setNegativeButton(canceltitle, dialogClickListener).setNeutralButton(remindTitle,dialogClickListener).show();
		}});
		
	}
}
