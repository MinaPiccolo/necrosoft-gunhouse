using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SonyNpUtilities : IScreen
{
	MenuLayout m_Menu;
	SonyNpTicketing m_Ticketing;
	SonyNpDialogs m_Dialogs;

	public SonyNpUtilities()
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

	Sony.NP.ErrorCode ErrorHandlerSystem(Sony.NP.ErrorCode errorCode = Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.System.GetLastError(out result);
			if (result.lastError != Sony.NP.ErrorCode.NP_OK)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
				return result.lastError;
			}
		}

		return errorCode;
	}

	public void Process(MenuStack stack)
	{
		MenuUtilities(stack);
	}

	public void Initialize()
    {
	    m_Menu = new MenuLayout(this, 450, 34);
		m_Ticketing = new SonyNpTicketing();
		m_Dialogs = new SonyNpDialogs();
		
		Sony.NP.System.OnGotBandwidth += OnSystemGotBandwidth;
		Sony.NP.System.OnGotNetInfo += OnSystemGotNetInfo;
		Sony.NP.System.OnNetInfoError += OnNetInfoError;

        Sony.NP.WordFilter.OnCommentCensored += OnWordFilterCensored;
        Sony.NP.WordFilter.OnCommentNotCensored += OnWordFilterNotCensored;
        Sony.NP.WordFilter.OnCommentSanitized += OnWordFilterSanitized;
		Sony.NP.WordFilter.OnWordFilterError += OnWordFilterError;
    }

    public void MenuUtilities(MenuStack menuStack)
    {
		m_Menu.Update();

		if (m_Menu.AddItem("Get Network Time", Sony.NP.System.IsConnected))
		{
			DateTime nowNet = new DateTime(Sony.NP.System.GetNetworkTime(), DateTimeKind.Utc);
			OnScreenLog.Add("networkTime: " + nowNet.ToLongDateString() + " - " + nowNet.ToLongTimeString());
		}
		
		if (m_Menu.AddItem("Bandwidth", Sony.NP.System.IsConnected && !Sony.NP.System.RequestBandwidthInfoIsBusy()))
        {
			ErrorHandlerSystem(Sony.NP.System.RequestBandwidthInfo());
        }

		if (m_Menu.AddItem("Net Info", !Sony.NP.System.RequestBandwidthInfoIsBusy()))
		{
			ErrorHandlerSystem(Sony.NP.System.RequestNetInfo());
		}

		if (m_Menu.AddItem("Net Device Type"))
		{
			Sony.NP.System.NetDeviceType deviceType = Sony.NP.System.GetNetworkDeviceType();
			OnScreenLog.Add("Network device: " + deviceType);
		}

		// The following features are only available when the user is signed into PSN.
		if (Sony.NP.User.IsSignedInPSN)
		{
#if !UNITY_PS3  // No NP dialogs on PS3
			if (m_Menu.AddItem("Dialogs"))
			{
				menuStack.PushMenu(m_Dialogs.GetMenu());
			}
#endif

#if UNITY_PSP2 || UNITY_PS3 || UNITY_PS4
			if (m_Menu.AddItem("Auth Ticketing"))
			{
				menuStack.PushMenu(m_Ticketing.GetMenu());
			}
#endif

			if (m_Menu.AddItem("Censor Bad Comment", Sony.NP.System.IsConnected && !Sony.NP.WordFilter.IsBusy()))
			{
				Sony.NP.WordFilter.CensorComment("Censor a shit comment");
			}

			if (m_Menu.AddItem("Sanitize Bad Comment", Sony.NP.System.IsConnected && !Sony.NP.WordFilter.IsBusy()))
			{
				Sony.NP.WordFilter.SanitizeComment("Sanitize a shit comment");
			}
		}

		if (m_Menu.AddBackIndex("Back"))
        {
            menuStack.PopMenu();
        }
    }

    void OnSystemGotBandwidth(Sony.NP.Messages.PluginMessage msg)
    {
        Sony.NP.System.Bandwidth bandwidth = Sony.NP.System.GetBandwidthInfo();
        OnScreenLog.Add("bandwidth download : " + bandwidth.downloadBPS / 8192.0f + " KBs");
        OnScreenLog.Add("bandwidth upload : " + bandwidth.uploadBPS / 8192.0f + " KBs");
    }

	void OnSystemGotNetInfo(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.System.NetInfoBasic info = Sony.NP.System.GetNetInfo();
		OnScreenLog.Add("Got Net info");
		OnScreenLog.Add(" Connection status: " + info.connectionStatus);
		OnScreenLog.Add(" IP address: " + info.ipAddress);
		OnScreenLog.Add(" NAT type: " + info.natType);
		OnScreenLog.Add(" NAT stun status: " + info.natStunStatus);
		OnScreenLog.Add(" NAT mapped addr: 0x" + info.natMappedAddr.ToString("X8"));
	}

	void OnNetInfoError(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandlerSystem();
	}

    void OnWordFilterCensored(Sony.NP.Messages.PluginMessage msg)
    {
        Sony.NP.WordFilter.FilteredComment result = Sony.NP.WordFilter.GetResult();
        OnScreenLog.Add("Censored: changed=" + result.wasChanged + ", comment='" + result.comment + "'");
    }

    void OnWordFilterNotCensored(Sony.NP.Messages.PluginMessage msg)
    {
        Sony.NP.WordFilter.FilteredComment result = Sony.NP.WordFilter.GetResult();
        OnScreenLog.Add("Not censored: changed=" + result.wasChanged + ", comment='" + result.comment + "'");
    }

    void OnWordFilterSanitized(Sony.NP.Messages.PluginMessage msg)
    {
        Sony.NP.WordFilter.FilteredComment result = Sony.NP.WordFilter.GetResult();
        OnScreenLog.Add("Sanitized: changed=" + result.wasChanged + ", comment='" + result.comment + "'");
    }

	void OnWordFilterError(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.WordFilter.GetLastError(out result);
		OnScreenLog.Add(result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
	}
}
