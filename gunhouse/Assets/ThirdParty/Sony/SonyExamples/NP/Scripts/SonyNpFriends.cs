using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SonyNpFriends : IScreen
{
	MenuLayout m_MenuFriends;

	public SonyNpFriends()
	{
		Initialize();
	}
    
	public MenuLayout GetMenu()
	{
		return m_MenuFriends;
	}

	public void OnEnter()
	{
	}

	public void OnExit()
	{
	}

	Sony.NP.ErrorCode ErrorHandlerFriends(Sony.NP.ErrorCode errorCode=Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.Friends.GetLastError(out result);
			if(result.lastError != Sony.NP.ErrorCode.NP_OK)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
				return result.lastError;
			}
		}

		return errorCode;
	}

	Sony.NP.ErrorCode ErrorHandlerPresence(Sony.NP.ErrorCode errorCode=Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.User.GetLastPresenceError(out result);
			if(result.lastError != Sony.NP.ErrorCode.NP_OK)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
				return result.lastError;
			}
		}

		return errorCode;
	}

#if !UNITY_PSP2
	Sony.NP.ErrorCode ErrorHandlerFacebook(Sony.NP.ErrorCode errorCode = Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.Facebook.GetLastError(out result);
			if (result.lastError != Sony.NP.ErrorCode.NP_OK)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
				return result.lastError;
			}
		}

		return errorCode;
	}
#endif

	public void Process(MenuStack stack)
	{
		MenuFriends(stack);
	}

	public void Initialize()
	{
	    m_MenuFriends = new MenuLayout(this, 550, 34);
		
		Sony.NP.Friends.OnFriendsListUpdated += OnFriendsListUpdated;
		Sony.NP.Friends.OnFriendsPresenceUpdated += OnFriendsListUpdated;
		Sony.NP.Friends.OnGotFriendsList += OnFriendsGotList;
		Sony.NP.Friends.OnFriendsListError += OnFriendsListError;
		Sony.NP.User.OnPresenceSet += OnSomeEvent;
		Sony.NP.User.OnPresenceError += OnPresenceError;

#if !UNITY_PSP2
        Sony.NP.Facebook.OnFacebookDialogStarted += OnSomeEvent;
        Sony.NP.Facebook.OnFacebookDialogFinished += OnSomeEvent;
        Sony.NP.Facebook.OnFacebookMessagePosted += OnSomeEvent;
		Sony.NP.Facebook.OnFacebookMessagePostFailed += OnFacebookMessagePostFailed;
#endif
	}

    static int friendsOffset = 0; // The current offset for a list of fiends to retrieve.
    static int friendsLimit = 10; // The maximum number of friends to return

    public void MenuFriends(MenuStack menuStack)
	{
        m_MenuFriends.Update();

        // Shouldn't really get all friends at once. This can cause issues when trying to fetch 2000 friends and can cause a large stall. 
        // Need to really use the RequestFriendsList version that returns a sub-set of friends.

        //if (m_MenuFriends.AddItem("Request All Friends", Sony.NP.User.IsSignedInPSN && !Sony.NP.Friends.FriendsListIsBusy()))
        //{
        //	// Get all friends.
        //	ErrorHandlerFriends(Sony.NP.Friends.RequestFriendsList());
        //}

		if (m_MenuFriends.AddItem("Request Some Friends", Sony.NP.User.IsSignedInPSN && !Sony.NP.Friends.FriendsListIsBusy()))
		{
            // Get friends specifying a limit for the number of friends to get, also lets you
            // filter which friends to get, e.g, only friends who are currently on-line, so, in
            // the call below we request the first 4 friends (offset = 0 and limit = 4), and only
            // include friends who are on-line (flags = Sony.NP.Friends.FlagNpFriendsListRequest.NP_FRIENDS_LIST_ONLINE).
            OnScreenLog.Add("Fetching Friends: " + friendsOffset + " To " + (friendsOffset+ friendsLimit-1));

            ErrorHandlerFriends(Sony.NP.Friends.RequestFriendsList(friendsOffset, friendsLimit, Sony.NP.Friends.FlagNpFriendsListRequest.NP_FRIENDS_LIST_ALL));
            friendsOffset += friendsLimit;
        }

		if (m_MenuFriends.AddItem("Set Presence", Sony.NP.User.IsSignedInPSN && !Sony.NP.User.OnlinePresenceIsBusy()))
		{
			ErrorHandlerPresence(Sony.NP.User.SetOnlinePresence("Testing UnityNpToolkit"));
		}

        if (m_MenuFriends.AddItem("Clear Presence", Sony.NP.User.IsSignedInPSN && !Sony.NP.User.OnlinePresenceIsBusy()))
		{
			ErrorHandlerPresence(Sony.NP.User.SetOnlinePresence(""));
		}

#if !UNITY_PSP2
        if (m_MenuFriends.AddItem("Post On Facebook", Sony.NP.User.IsSignedInPSN && !Sony.NP.Facebook.IsBusy()))
        {
            // To use Facebook integration you need to visit https://developers.facebook.com/ and create
            // an app which binds to your Sony applications title ID, this gets you an app ID which is used below.

            Sony.NP.Facebook.PostFacebook message = new Sony.NP.Facebook.PostFacebook();
            message.appID = 701792156521339;
            message.userText = "I'm testing Unity's facebook integration !";
            message.photoURL = "http://uk.playstation.com/media/RZXT_744/159/PlayStationNetworkFeaturedImage.jpg";
            message.photoTitle = "Title";
            message.photoCaption = "This is the caption";
            message.photoDescription = "This is the description";
            message.actionLinkName = "Go To Unity3d.com";
            message.actionLinkURL = "http://unity3d.com/";
            ErrorHandlerFacebook(Sony.NP.Facebook.PostMessage(message));
        }
#endif

        if (m_MenuFriends.AddBackIndex("Back"))
		{
			menuStack.PopMenu();
		}
	}

#if !UNITY_PSP2
	void OnFacebookMessagePostFailed(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandlerFacebook();
	}
#endif

	void OnSomeEvent(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Event: " + msg.type);
	}

	void OnFriendsListUpdated(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Requesting Friends List for Event: " + msg.type);
		ErrorHandlerFriends(Sony.NP.Friends.RequestFriendsList());
	}

	string OnlinePresenceType(Sony.NP.Friends.EnumNpPresenceType type)
	{
		switch (type)
		{
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_UNKNOWN:
				return "unknown";
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_NONE:
				return "none";
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_DEFAULT:
				return "default";
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_GAME_JOINING:
				return "joining";
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_GAME_JOINING_ONLY_FOR_PARTY:
				return "joining party";
			case Sony.NP.Friends.EnumNpPresenceType.IN_GAME_PRESENCE_TYPE_JOIN_GAME_ACK:
				return "join ack";
		}

		return "unknown";
	}

	string OnlineStatus(Sony.NP.Friends.EnumNpOnlineStatus status)
	{
		switch (status)
		{
			case Sony.NP.Friends.EnumNpOnlineStatus.ONLINE_STATUS_OFFLINE:
				return "offline";
			case Sony.NP.Friends.EnumNpOnlineStatus.ONLINE_STATUS_AFK:
				return "afk";
			case Sony.NP.Friends.EnumNpOnlineStatus.ONLINE_STATUS_ONLINE:
				return "online";
		}
		return "unknown";
	}

	void OnFriendsGotList(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Got Friends List!");
		Sony.NP.Friends.Friend[] friends = Sony.NP.Friends.GetCachedFriendsList();
		foreach (Sony.NP.Friends.Friend friend in friends)
		{
#if UNITY_PS3
			OnScreenLog.Add(friend.npOnlineID + " - " + " - " + friend.npComment + " - " + friend.npPresenceTitle + " - " + friend.npPresenceStatus);
#else
			string npID = System.Text.Encoding.Default.GetString(friend.npID);
			OnScreenLog.Add(friend.npOnlineID
						+ ", np(" + npID + ")"
						+ ", os(" + OnlineStatus(friend.npOnlineStatus) + ")"
						+ ", pt(" + OnlinePresenceType(friend.npPresenceType) + ")"
						+ ", prsc(" + friend.npPresenceTitle + ", " + friend.npPresenceStatus + ")"
						+ "," + friend.npComment );
#endif
        }

        if (friends.Length < friendsLimit && friendsOffset != 0)
        {
            friendsOffset = 0;
        }
    }

	void OnFriendsListError(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandlerFriends();
	}

	void OnPresenceError(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandlerPresence();
	}
}
