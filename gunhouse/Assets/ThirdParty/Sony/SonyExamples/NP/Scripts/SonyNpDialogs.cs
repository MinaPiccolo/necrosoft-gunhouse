using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SonyNpDialogs : IScreen
{
	MenuLayout m_Menu;

	public SonyNpDialogs()
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
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.Dialogs.GetLastError(out result);
			OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
		}

		return errorCode;
	}

	public void Process(MenuStack stack)
	{
		m_Menu.Update();

#if !UNITY_PS3  // No NP dialogs on PS3
		bool enableDialogs = Sony.NP.User.IsSignedInPSN && !Sony.NP.Dialogs.IsDialogOpen;
		if (m_Menu.AddItem("Friends Dialog", enableDialogs))
		{
			ErrorHandler(Sony.NP.Dialogs.FriendsList());
		}

		if (m_Menu.AddItem("Shared History Dialog", enableDialogs))
		{
			ErrorHandler(Sony.NP.Dialogs.SharedPlayHistory());
		}

		if (m_Menu.AddItem("Profile Dialog", enableDialogs))
		{
			Sony.NP.User.UserProfile profile = Sony.NP.User.GetCachedUserProfile();
			ErrorHandler(Sony.NP.Dialogs.Profile(profile.npID));
		}
#if UNITY_PS4
		if (m_Menu.AddItem("Purchase Plus dialog", enableDialogs))
		{
			ErrorHandler(Sony.NP.Dialogs.Commerce(Sony.NP.Dialogs.CommerceDialogMode.PLUS, Sony.NP.Requests.PlusFeature.REALTIME_MULTIPLAY));
		}	
#endif
#endif

		if (m_Menu.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}

	public void Initialize()
	{
		m_Menu = new MenuLayout(this, 450, 34);

#if !UNITY_PS3  // No NP dialogs on PS3
		Sony.NP.Dialogs.OnDlgFriendsListClosed += OnFriendDialogClosed;
		Sony.NP.Dialogs.OnDlgSharedPlayHistoryClosed += OnSharedPlayHistoryDialogClosed;
		Sony.NP.Dialogs.OnDlgProfileClosed += OnProfileDialogClosed;
		Sony.NP.Dialogs.OnDlgCommerceClosed += OnCommerceDialogClosed;
#endif
	}

#if !UNITY_PS3  // No NP dialogs on PS3
	void OnFriendDialogClosed(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.Dialogs.NpDialogReturn result = Sony.NP.Dialogs.GetDialogResult();
		OnScreenLog.Add("Friends Dialog closed with result: " + result.result);
		if (result.result == Sony.NP.Dialogs.EnumNpDlgResult.NP_DLG_OK)
		{
			// For demo purposes, use the NpID from the dialog result to open the profile dialog.
			Sony.NP.Dialogs.Profile(result.npID);
		}
	}

	void OnSharedPlayHistoryDialogClosed(Sony.NP.Messages.PluginMessage msg)
	{
		// For demo purposes, use the NpID from the dialog result to open the profile dialog.
		Sony.NP.Dialogs.NpDialogReturn result = Sony.NP.Dialogs.GetDialogResult();
		OnScreenLog.Add("Shared play history dialog closed with result: " + result.result);
		if (result.result == Sony.NP.Dialogs.EnumNpDlgResult.NP_DLG_OK)
		{
			// For demo purposes, use the NpID from the dialog result to open the profile dialog.
			Sony.NP.Dialogs.Profile(result.npID);
		}
	}

	void OnProfileDialogClosed(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.Dialogs.NpDialogReturn result = Sony.NP.Dialogs.GetDialogResult();
		OnScreenLog.Add("Profile dialog closed with result: " + result.result);
	}

	void OnCommerceDialogClosed(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.Dialogs.NpDialogReturn result = Sony.NP.Dialogs.GetDialogResult();
		OnScreenLog.Add("Commerce dialog closed with result: " + result.result + " PlusAllowed:" + result.plusAllowed );
	}
	
	
#endif
}
