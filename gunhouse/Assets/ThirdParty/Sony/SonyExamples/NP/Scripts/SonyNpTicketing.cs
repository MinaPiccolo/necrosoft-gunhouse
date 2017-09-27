using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SonyNpTicketing : IScreen
{
	MenuLayout m_MenuTicketing;
	bool m_GotTicket = false;
	Sony.NP.Ticketing.Ticket m_Ticket;

	public SonyNpTicketing()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_MenuTicketing;
	}

	public void Initialize()
	{
		m_MenuTicketing = new MenuLayout(this, 560, 34);

		Sony.NP.Ticketing.OnGotTicket += OnGotTicket;
#if UNITY_PS4
        Sony.NP.Ticketing.OnGotAccessToken += OnGotAccessToken;
#endif
#if UNITY_PSP2
        Sony.NP.Ticketing.OnGotAuthorizationCode += OnGotAuthorizationCode;
#endif
        Sony.NP.Ticketing.OnError += OnError;
	}

	public void OnEnter()
	{
    }

    public void OnExit()
	{
	}

	Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode = Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.Ticketing.GetLastError(out result);
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
		m_MenuTicketing.Update();

		bool ready = Sony.NP.User.IsSignedInPSN && !Sony.NP.Ticketing.IsBusy();

#if UNITY_PSP2
        if (m_MenuTicketing.AddItem("Request Authorization Code", ready))
        {
            // Test values for cliendId and scope are from the SDK nptoolkit sample ... replace with your own project values
            String clientId = "c8c483e7-f0b4-420b-877b-307fcb4c3cdc";
            String scope = "psn:s2s";
            ErrorHandler(Sony.NP.Ticketing.RequestAuthorizationCode(clientId, scope));
        }
#endif

#if UNITY_PS4
		if (m_MenuTicketing.AddItem("Request User Access code", ready))
		{
			// test values from SDK nptoolkit sample ... replace with your own project values
			String clientId = "c8c483e7-f0b4-420b-877b-307fcb4c3cdc";
			String scope = "psn:s2s";
			ErrorHandler(Sony.NP.Ticketing.RequestUserAccesscode(clientId, scope));
		}
#endif

        if (m_MenuTicketing.AddItem("Request Ticket", ready))
        {
            ErrorHandler(Sony.NP.Ticketing.RequestTicket());
        }

#if !UNITY_PS3 // Not supported on PS3.
        if (m_MenuTicketing.AddItem("Request Cached Ticket", ready))
		{
			ErrorHandler(Sony.NP.Ticketing.RequestCachedTicket());
		}
#endif

		if (m_MenuTicketing.AddItem("Get Ticket Entitlements", m_GotTicket))
		{
			Sony.NP.Ticketing.TicketEntitlement[] entitlements = Sony.NP.Ticketing.GetTicketEntitlements(m_Ticket);

			OnScreenLog.Add("Ticket contains " + entitlements.Length + " entitlements");
			for (int i = 0; i < entitlements.Length; i++)
			{
				OnScreenLog.Add("Entitlement " + i);
				OnScreenLog.Add(" " + entitlements[i].id + " rc: " + entitlements[i].remainingCount + " cc: " + entitlements[i].consumedCount + " type: " + entitlements[i].type);
			}
		}
		
		if (m_MenuTicketing.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}

	void OnGotTicket(Sony.NP.Messages.PluginMessage msg)
	{
		m_Ticket = Sony.NP.Ticketing.GetTicket();
		m_GotTicket = true;
		OnScreenLog.Add("GotTicket");
		OnScreenLog.Add(" dataSize: " + m_Ticket.dataSize);

		Sony.NP.Ticketing.TicketInfo info = Sony.NP.Ticketing.GetTicketInfo(m_Ticket);
		OnScreenLog.Add(" Issuer ID: " + info.issuerID);
		DateTime it = new DateTime(info.issuedDate, DateTimeKind.Utc);
		OnScreenLog.Add(" Issue date: " + it.ToLongDateString() + " - " + it.ToLongTimeString());
		DateTime et = new DateTime(info.expireDate, DateTimeKind.Utc);
		OnScreenLog.Add(" Expire date: " + et.ToLongDateString() + " - " + et.ToLongTimeString());
		OnScreenLog.Add(" Account ID: 0x" + info.subjectAccountID.ToString("X8"));
		OnScreenLog.Add(" Online ID: " + info.subjectOnlineID);
		OnScreenLog.Add(" Service ID: " + info.serviceID);
		OnScreenLog.Add(" Domain: " + info.subjectDomain);
		OnScreenLog.Add(" Country Code: " + info.countryCode);
		OnScreenLog.Add(" Language Code: " + info.languageCode);
		OnScreenLog.Add(" Age: " + info.subjectAge);
		OnScreenLog.Add(" Chat disabled: " + info.chatDisabled);
		OnScreenLog.Add(" Content rating: " + info.contentRating);
	}

#if UNITY_PSP2
    void OnGotAuthorizationCode(Sony.NP.Messages.PluginMessage msg)
    {
        OnScreenLog.Add("OnGotAuthorizationCode");
        Sony.NP.Ticketing.AuthorizationCode authCode = Sony.NP.Ticketing.GetAuthorizationCode();
        OnScreenLog.Add(" AuthCode: " + authCode.authCode);
        OnScreenLog.Add(" Issuer ID: " + authCode.issuerID);
    }
#endif

#if UNITY_PS4
    void OnGotAccessToken(Sony.NP.Messages.PluginMessage msg)
	{
		m_Ticket = Sony.NP.Ticketing.GetTicket();
		m_GotTicket = true;
		OnScreenLog.Add("OnGotAccessToken");
		OnScreenLog.Add(" dataSize: " + m_Ticket.dataSize);

		string authorizationCode = System.Text.Encoding.Default.GetString(m_Ticket.data);
		OnScreenLog.Add(" AccessToken AuthorizationCode: " + authorizationCode);
	}
#endif



    void OnError(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandler();
	}
}
