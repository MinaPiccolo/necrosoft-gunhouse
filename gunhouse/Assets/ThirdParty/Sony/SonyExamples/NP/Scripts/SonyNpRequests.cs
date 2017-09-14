using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_PS4

public class SonyNpRequests : IScreen
{
	MenuLayout m_Menu;

	public SonyNpRequests()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_Menu;
	}

	public void OnEnter()
	{
	}

	public void OnExit()
	{
	}

	Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			OnScreenLog.Add("Error: " + errorCode);
		}

		return errorCode;
	}

	string m_onlineID;
	
	// return a suitable OnlineID to demonstrate nptoolkit requests
	// requests a default user profile if one has not already been generated
	private string getDemoOnlineID()
	{
		if (String.IsNullOrEmpty(m_onlineID))
		{
			m_onlineID = Sony.NP.User.GetCachedUserProfile().onlineID;
			if (String.IsNullOrEmpty(m_onlineID))
			{
				Sony.NP.User.RequestUserProfile();
				// really we should wait for a succesful result from Sony.NP.User.OnGotUserProfile() here, but for simplicity we just display a message
				OnScreenLog.Add("No demo online ID has been set, attempting to retreive online ID from default user profile... Please wait for result and retry");
			}
		}
		
		return m_onlineID;
	}

	public void Process(MenuStack stack)
	{
		m_Menu.Update();

		if (m_Menu.AddItem("PlayStationPlus check", Sony.NP.User.IsSignedInPSN))
		{
			Sony.NP.Requests.PlusFeature features = Sony.NP.Requests.PlusFeature.REALTIME_MULTIPLAY;
			ErrorHandler(Sony.NP.Requests.CheckPlus(features));		// uses default user, result is returned in OnCheckPlusResult()
		}
		if (m_Menu.AddItem("PlayStationPlus check all", Sony.NP.User.IsSignedInPSN))		// check all local users
		{
			Sony.NP.Requests.PlusFeature features = Sony.NP.Requests.PlusFeature.REALTIME_MULTIPLAY;

			for (int slot = 0; slot < 4; slot++)
			{
				UnityEngine.PS4.PS4Input.LoggedInUser userdata = UnityEngine.PS4.PS4Input.PadGetUsersDetails( slot );
				if (userdata.userId != -1)
				{
					ErrorHandler(Sony.NP.Requests.CheckPlus(userdata.userId, features)); // results are returned in OnCheckPlusResult()
				}
			}
		}

		if (m_Menu.AddItem("Notify Plus realtime", Sony.NP.User.IsSignedInPSN))
		{
			// this needs to be called everyframe (or at least once a second)
			Sony.NP.Requests.PlusFeature features = Sony.NP.Requests.PlusFeature.REALTIME_MULTIPLAY;
			ErrorHandler(Sony.NP.Requests.NotifyPlusFeature(features));		// uses default user ... 
		}
		
		if (m_Menu.AddItem("Notify Plus async", Sony.NP.User.IsSignedInPSN))
		{
			Sony.NP.Requests.PlusFeature features = Sony.NP.Requests.PlusFeature.ASYNC_MULTIPLAY;
			ErrorHandler(Sony.NP.Requests.NotifyPlusFeature(features));		// uses default user
		}
		
		if (m_Menu.AddItem("GetAccountLanguage", Sony.NP.User.IsSignedInPSN))
		{
			int userId = 0;	// user id to check [0 indicates current user set by Sony.NP.User.SetCurrentUserId() ]
			Sony.NP.ErrorCode result = Sony.NP.Requests.GetAccountLanguage(userId);		// result is returned through Sony.NP.Requests.OnAccountLanguageResult
			OnScreenLog.Add("async GetAccountLanguage() started ... initial result code : " + result);
		}
		if (m_Menu.AddItem("GetParentalControlInfo", Sony.NP.User.IsSignedInPSN))
		{
			int userId = 0;	// user id to check [0 indicates current user set by Sony.NP.User.SetCurrentUserId() ]
			Sony.NP.ErrorCode result = Sony.NP.Requests.GetParentalControlInfo(userId);		// result is returned through Sony.NP.Requests.OnParentalControlResult
			OnScreenLog.Add("async GetParentalControlInfo() started ... initial result code : " + result);
		}
		if (m_Menu.AddItem("CheckNpAvailability", Sony.NP.User.IsSignedInPSN))
		{
			int userId = 0;	// user id to check [0 indicates current user set by Sony.NP.User.SetCurrentUserId() ]
			Sony.NP.ErrorCode result = Sony.NP.Requests.CheckNpAvailability(userId);		// result is returned through Sony.NP.Requests.OnCheckNpAvailabilityResult
			OnScreenLog.Add("async CheckNpAvailability() started ... initial result code : " + result);
		}
		if (m_Menu.AddItem("SetGamePresenceOnline", Sony.NP.User.IsSignedInPSN))
		{
			Sony.NP.ErrorCode result = Sony.NP.Requests.SetGamePresenceOnline(getDemoOnlineID());
			OnScreenLog.Add("SetGamePresenceOnline result : " +  result);
		}		
		  
		if (m_Menu.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}

	public void Initialize()
	{
		m_Menu = new MenuLayout(this, 450, 34);
		Sony.NP.Requests.OnCheckPlusResult += OnCheckPlusResult;
		Sony.NP.Requests.OnAccountLanguageResult += OnAccountLanguageResult;
		Sony.NP.Requests.OnParentalControlResult += OnParentalControlResult;
		Sony.NP.Requests.OnCheckNpAvailabilityResult += OnCheckNpAvailabilityResult;
	}


	void OnCheckPlusResult(Sony.NP.Messages.PluginMessage msg)
	{
//		byte[] result = Sony.NP.Requests.GetRequestResultData(msg);
//		OnScreenLog.Add("result as hex : " + result.Length +  " : " +  BitConverter.ToString(result));

		bool checkresult;
		int userId;
		Sony.NP.Requests.GetCheckPlusResult(msg, out checkresult, out userId);
		OnScreenLog.Add("OnPlusCheckResult  returned:" + checkresult + " userId :0x" + userId.ToString("X"));
	}

	void OnAccountLanguageResult(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("OnAccountLanguageResult  AccountLanguage:" + Sony.NP.Requests.GetAccountLanguageResult(msg) );
	}

	void OnParentalControlResult(Sony.NP.Messages.PluginMessage msg)
	{
		int Age;
		bool chatRestriction;
		bool ugcRestriction;
		Sony.NP.Requests.GetParentalControlInfoResult(msg, out Age, out chatRestriction, out ugcRestriction);
		OnScreenLog.Add("OnParentalControlResult  Age:" + Age + " chatRestriction:" + chatRestriction + " ugcRestriction:" + ugcRestriction );
	}

	void OnCheckNpAvailabilityResult(Sony.NP.Messages.PluginMessage msg)
	{
		int requestResult = Sony.NP.Requests.GetRequestResult(msg);
		OnScreenLog.Add("OnCheckNpAvailabilityResult  result:" + requestResult);
	}
	
}

#endif // UNITY_PS4
