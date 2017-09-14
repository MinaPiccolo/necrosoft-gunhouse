using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SonyNpMain : MonoBehaviour, IScreen
{
	MenuStack m_MenuStack = null;
	MenuLayout m_MenuMain;
	bool m_NpReady = false;		// Is the NP plugin initialised and ready for use.
	SonyNpUser m_User;
	SonyNpFriends m_Friends;
	SonyNpTrophy m_Trophies;
	SonyNpRanking m_Ranking;
	SonyNpSession m_Sessions;
	int m_SendCount = 0;
	float m_SendingInterval = 1;
	SonyNpMessaging m_Messaging;
	SonyNpCloud m_CloudStorage;
	SonyNpUtilities m_Utilities;
	SonyNpCommerce m_Commerce;
#if UNITY_PS4
	SonyNpRequests m_Requests;
#endif

#if NPTOOLKIT_CROSS_PLATFORM_TROPHIES || NPTOOLKIT_CROSS_PLATFORM_MATCHING
	// Cross-platform Application Guide.
	// https://psvita.scedev.net/docs/vita-en,Cross_Platform_Application-Guide-vita/
	// https://psvita.scedev.net/projects/network/dl/dl/1143/6128/PSN_Service_Setup-Guide_e.pdf
	//
	// Also see also
	// https://ps4.siedev.net/projects/sdk/dl/dl/270/9479/PSN_Service_Setup-Supplement_e.pdf
	// 
	// Cross-platform Trophies
	// https://psvita.scedev.net/docs/vita-en,Cross_Platform_Application-Guide-vita,Cross-Platform_Trophy_System/1/
	// 
	// When creating trophy packs for cross-platform trophies in the Sony Trophy Pack File Utility make sure that you...
	// 
	// Set the supported platforms to include all of the platforms that will be sharing trophies, e.g. PSVita and PS4.
	// 
	// For the PSVita trophy pack set the NP Comms ID to the same shared ID, e.g. the trophy ID label that
	// the PS4 uses.
	// 
	// Then in your script code register the NP comms ID, pass phrase and signature for the trophy service before the
	// calling Sony.NP.Trophies.RegisterTrophyPack, (shown below)
	// 
	// Other Services.
	// 
	// You can also setup other cross-platform services, e.g. TUS, TSS and Ranking, to do this you would call RegisterCommsID
	// as shown below but change the serviceType param to whichever service you require. You also need to ask SCE to enable
	// cross-platform for any services you might want to this way by asking in your service requests thread on dev-net
	// 
	// Cross-platform Storage (TUS/TSS)
	// https://psvita.scedev.net/docs/vita-en,Cross_Platform_Application-Guide-vita,Cross-Platform_Online_Storage/
	// 
	// Cross-platform Score Ranking
	// https://psvita.scedev.net/docs/vita-en,Cross_Platform_Application-Guide-vita,Cross-Platform_Ranking/1/
	// For cross-platform services, e.g. sharing trophies across PS Vita and PS4 using the PS4's NP Communications ID...
#endif

	struct Avatar
	{
		public Avatar(GameObject gameObject)
		{
			this.gameObject = gameObject;
			url = "";
			pendingDownload = false;
			texture = null;
		}
		public string url;
		public bool pendingDownload;
		public Texture2D texture;
		public GameObject gameObject;
	};

	static Avatar[] m_Avatars = new Avatar[2];
	static public Texture2D m_AvatarTexture = null;

	// Class containing some data and methods for reading/writing from and to a byte buffer, the data could be almost anything.
	public struct SharedSessionData
	{
		public int id;
		public string text;
		public int item1;
		public int item2;

		public byte[] WriteToBuffer()
		{
			System.IO.MemoryStream output = new MemoryStream();
			System.IO.BinaryWriter writer = new BinaryWriter(output);
			writer.Write(id);
			writer.Write(text);
			writer.Write(item1);
			writer.Write(item2);
			writer.Close();
			return output.ToArray();
		}

		public void ReadFromBuffer(byte[] buffer)
		{
			System.IO.MemoryStream input = new MemoryStream(buffer);
			System.IO.BinaryReader reader = new BinaryReader(input);
			id = reader.ReadInt32();
			text = reader.ReadString();
			item1 = reader.ReadInt32();
			item2 = reader.ReadInt32();
			reader.Close();
		}
	}

	static public Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			OnScreenLog.Add("Error: " + errorCode);
		}

		return errorCode;
	}

	void Start()
	{
		m_Avatars[0] = new Avatar(GameObject.Find("UserAvatar"));
		m_Avatars[1] = new Avatar(GameObject.Find("RemoteUserAvatar"));

#if UNITY_PS4
		m_MenuMain = new MenuLayout(this, 600, 34);
#else
		m_MenuMain = new MenuLayout(this, 500, 34);
#endif
		m_MenuStack = new MenuStack();
		m_MenuStack.SetMenu(m_MenuMain);

		// Register a callback for completion of NP initialization.
		Sony.NP.Main.OnNPInitialized += OnNPInitialized;

		// Initialize the NP Toolkit.
		OnScreenLog.Add("Initializing NP");

#if UNITY_PS4
		OnScreenLog.Add(System.String.Format("Initial UserId:0x{0:X}  Primary UserId:0x{1:X}", UnityEngine.PS4.Utility.initialUserId, UnityEngine.PS4.Utility.primaryUserId));
		
		// When a user logs out of the machine, we receive an event from the system. We must inform the NpToolkit library that the user has gone by calling LogOutUser
		UnityEngine.PS4.PS4Input.OnUserServiceEvent = ((uint eventtype, uint userid) => 
		{
			int SCE_USER_SERVICE_EVENT_TYPE_LOGOUT = 1;
			if (eventtype == SCE_USER_SERVICE_EVENT_TYPE_LOGOUT)
				Sony.NP.User.LogOutUser((int)userid);
			
			OnScreenLog.Add(System.String.Format("OnUserServiceEvent event:{0} userid:0x{1:X}",
				eventtype==SCE_USER_SERVICE_EVENT_TYPE_LOGOUT?"LOGOUT":"LOGIN", 
				userid));
		} );
#endif
		
		// Enable/Disable internal logging, log messages are handled by the OnLog, OnLogWarning and OnLogError event handlers.
		Sony.NP.Main.enableInternalLogging = true;

		// Add NP event handlers.
		Sony.NP.Main.OnLog += OnLog;
		Sony.NP.Main.OnLogWarning += OnLogWarning;
		Sony.NP.Main.OnLogError += OnLogError;

#if NPTOOLKIT_CROSS_PLATFORM_TROPHIES
		// Initialization for cross-platform trophies...
		//
		// By default Sony.NP.Main.Initialize performs trophy registration, however, if you intend to use cross-platform
		// trophies then trophy registration must be done separately after first registering a cross-platform npCommunications ID
		// for the trophy service (see the InitializeCrossPlatformTrophies function below). To achieve this the
		// Sony.NP.Main.kNpToolkitCreate_DoNotInitializeTrophies should be set in the npCreationFlags that are passed into
		// Sony.NP.Main.Initialize to prevent automatic trophy installation.

		Sony.NP.Main.Initialize(Sony.NP.Main.kNpToolkitCreate_DoNotInitializeTrophies);
		InitializeCrossPlatformTrophies();
#elif NPTOOLKIT_NO_RANKING
		// If your application does not make use of NP ranking then you must specify the
		// Sony.NP.Main.kNpToolkitCreate_NoRanking flag when initializing otherwise you will be
		// in violation of "TRC R3002 - Title calls NpScore",
		//
		Sony.NP.Main.Initialize(Sony.NP.Main.kNpToolkitCreate_CacheTrophyIcons | Sony.NP.Main.kNpToolkitCreate_NoRanking);
#elif NPTOOLKIT_OVERRIDE_AGE_RATING
		// You can override the age rating like this...
		int npRatingAgeDefault = 12;
		int npRatingAgeAustralia = 15;
		int npRatingAgeNewzealand = 15;
		int npCreationFlags = Sony.NP.Main.kNpToolkitCreate_CacheTrophyIcons;
		Sony.NP.Main.AddNpAgeRatingRestriction( "au" , npRatingAgeAustralia ); // Specify country code and age
		Sony.NP.Main.AddNpAgeRatingRestriction( "nz" , npRatingAgeNewzealand); // Specify country code and age
		Sony.NP.Main.InitializeWithNpAgeRating( npCreationFlags , npRatingAgeDefault );
#else
		// Initialise with trophy registration and the age rating that was set in the editor player settings...
		Sony.NP.Main.Initialize(Sony.NP.Main.kNpToolkitCreate_CacheTrophyIcons);
#endif

#if NPTOOLKIT_CROSS_PLATFORM_MATCHING && UNITY_PS4
		// Note that you only need to do this on one platform, for example either call RegisterCommsID in the Vita app so that it can
		// share the PS4 service or call RegisterServiceLabel on the PS4 app so that it can share the PS Vita service. For this
		// example we will setup the service label if building on PS4 to share the service that was created for the PS Vita.

		// For PS4 we register the service label for the matching service that matches the np Comms ID of the PS Vita version of the application.
		ErrorHandler(Sony.NP.Main.RegisterServiceLabel(Sony.NP.NpServiceType.Matching, 1));
#endif

		// Session image must be set after initialization
#if UNITY_PSP2
		// If we want to we can register an alternative service ID for a service, e.g. if two titles need to share the same commerce store.
		//ErrorHandler(Sony.NP.Main.RegisterServiceID(Sony.NP.NpServiceType.Commerce, "ED1633-NPXB01864_00"));

		 // Some PS4 and PSP2 Np APIs require a session image, specifically for sending matching session invite messages.
		string sessionImage = Application.streamingAssetsPath + "/PSP2SessionImage.jpg";
		Sony.NP.Main.SetSessionImage(sessionImage);
#endif
#if UNITY_PS4
		// Some PS4 and PSP2 Np APIs require a session image, specifically for sending matching session invite messages.
		string sessionImage = Application.streamingAssetsPath + "/PS4SessionImage.jpg";
		Sony.NP.Main.SetSessionImage(sessionImage);
#endif

		// System events.
		Sony.NP.System.OnConnectionUp += OnSomeEvent;
		Sony.NP.System.OnConnectionDown += OnConnectionDown;
		Sony.NP.System.OnSysResume += OnSomeEvent;
		Sony.NP.System.OnSysNpMessageArrived += OnSomeEvent;
		Sony.NP.System.OnSysStorePurchase += OnSomeEvent;
		Sony.NP.System.OnSysStoreRedemption += OnSomeEvent;
		Sony.NP.System.OnSysEvent += OnSomeEvent;	// Some other event.
		
		// Messaging events.
		Sony.NP.Messaging.OnSessionInviteMessageRetrieved += OnMessagingSessionInviteRetrieved;
		Sony.NP.Messaging.OnMessageSessionInviteReceived += OnMessagingSessionInviteReceived;
		Sony.NP.Messaging.OnMessageSessionInviteAccepted += OnMessagingSessionInviteAccepted;
		Sony.NP.Messaging.OnMessagePlayTogetherReceived += OnPlayTogether;

		// User events.
		Sony.NP.User.OnSignedIn += OnSignedIn;
		Sony.NP.User.OnSignedOut += OnSomeEvent;
		Sony.NP.User.OnSignInError += OnSignInError;

		m_User = new SonyNpUser();
		m_Friends = new SonyNpFriends();
		m_Trophies = new SonyNpTrophy();
		m_Ranking = new SonyNpRanking();
		m_Sessions = new SonyNpSession();
		m_Messaging = new SonyNpMessaging();
		m_Commerce = new SonyNpCommerce();
		m_CloudStorage = new SonyNpCloud();
		m_Utilities = new SonyNpUtilities();
#if UNITY_PS4
		m_Requests = new SonyNpRequests();
#endif		

#if UNITY_PSP2
		// Test the upgradable/trial app flag.
		// Note that this only works with packages, when running PC Hosted skuFlags always equals 'None'.
		UnityEngine.PSVita.Utility.SkuFlags skuf = UnityEngine.PSVita.Utility.skuFlags;
		if (skuf == UnityEngine.PSVita.Utility.SkuFlags.Trial)
		{
			OnScreenLog.Add("Trial Mode, purchase the full app to get extra features.");
		}
#endif
	}

	void InitializeTrophies()
	{
		// Register the trophy pack.
		Sony.NP.Trophies.RegisterTrophyPack();
	}

#if NPTOOLKIT_CROSS_PLATFORM_TROPHIES
	// Initialize cross-platform trophies, in this example the PS Vita, PS4 all share the same trophy pack
	// using the PS4's NP Communications ID.
	void InitializeCrossPlatformTrophies()
	{
#if UNITY_PS4
		// Note that you only need to do this on one platform, for example either call RegisterCommsID in the Vita app so that it can
		// share the PS4 service or call RegisterServiceLabel on the PS4 app so that it can share the PS Vita service. For this
		// example we will setup the service label if building on PS4 to share the service that was created for the PS Vita.

		// For PS4 we register the service label for the trophy service that matches the np Comms ID of the PS Vita version of the application.
		ErrorHandler(Sony.NP.Main.RegisterServiceLabel(Sony.NP.NpServiceType.Trophy, 1));
#endif
		// Register the trophy pack.
		Sony.NP.Trophies.RegisterTrophyPack();
	}
#endif

	static public void SetAvatarURL(string url, int index)
	{
		m_Avatars[index].url = url;
		m_Avatars[index].pendingDownload = true;
	}

	IEnumerator DownloadAvatar(int index)
	{
		OnScreenLog.Add(" Downloading avatar image");

		m_Avatars[index].gameObject.GetComponent<GUITexture>().texture = null;
		m_Avatars[index].texture = new Texture2D(4, 4, TextureFormat.DXT1, false);

		// Start a download of the given URL
		var www = new WWW(m_Avatars[index].url);

		// Wait until the download is done
		yield return www;

		// assign the downloaded image to the main texture of the object
		www.LoadImageIntoTexture(m_Avatars[index].texture);

		if (www.bytesDownloaded == 0)
		{
			OnScreenLog.Add(" Error: " + www.error);
		}
		else
		{
			m_Avatars[index].texture.Apply(true, true);	// Release non-GPU texture memory.

			System.Console.WriteLine("w " + m_Avatars[index].texture.width + ", h " + m_Avatars[index].texture.height + ", f " + m_Avatars[index].texture.format);

			if (m_Avatars[index].texture != null)
			{
				m_Avatars[index].gameObject.GetComponent<GUITexture>().texture = m_Avatars[index].texture;
			}
		}
		OnScreenLog.Add(" Done");
	}

	void Update()
	{
		Sony.NP.Main.Update();

		if (m_Sessions.m_SendingData)
		{
			m_SendingInterval -= Time.deltaTime;
			if (m_SendingInterval <= 0)
			{
				SendSessionData();
				m_SendingInterval = 1;
			}
		}

		for (int i = 0; i < m_Avatars.Length; i++)
		{
			if (m_Avatars[i].pendingDownload)
			{
				m_Avatars[i].pendingDownload = false;
				StartCoroutine(DownloadAvatar(i));
			}
		}

		if (m_Sessions != null)
		{
			m_Sessions.Update();
		}
	}

	void OnNPInitialized(Sony.NP.Messages.PluginMessage msg)
	{
		m_NpReady = true;

#if UNITY_PS4
		// On PS4 we dont need to request a sign in ... it always happens externally from the application ... i.e. from the home screen
#else
		// If the game relied on online features then it would make sense to automatically sign in to PSN
		// here, but for the sake of a better example we'll use a menu item to sign in.
		//OnScreenLog.Add("Begin sign in");
		//Sony.NP.User.SignIn();        // NP has been fully initialized so it's now safe to sign in etc.
#endif
	}

	void MenuMain()
	{
		m_MenuMain.Update();

		bool signedIn = Sony.NP.User.IsSignedInPSN;

		if(m_NpReady)
		{
			if(!signedIn)
			{
#if UNITY_PS4
				// Indicate that the user is not signed in to PSN.
				m_MenuMain.AddItem("Not Signed In To PSN", false);
#else
				// Add a menu item for signing in to PSN.
				// Note that we could sign in automatically when OnNPInitialized is called if we
				// always require the user to be signed in, i.e. the game relies on online features.
				if (m_MenuMain.AddItem("Sign In To PSN", m_NpReady))
				{
					OnScreenLog.Add("Begin sign in");
					Sony.NP.User.SignIn();
				}
#endif
			}

			if (m_MenuMain.AddItem("Trophies"))
			{
				m_MenuStack.PushMenu(m_Trophies.GetMenu());
			}

			if (m_MenuMain.AddItem("User"))
			{
				m_MenuStack.PushMenu(m_User.GetMenu());
			}

#if UNITY_PS3
			if (m_MenuMain.AddItem("Utilities & Auth"))
#elif UNITY_PS4
			if (m_MenuMain.AddItem("Utilities & Dialogs"))
#else
			if (m_MenuMain.AddItem("Utilities, Dialogs & Auth"))
#endif
			{
				m_MenuStack.PushMenu(m_Utilities.GetMenu());
			}

			// The following features are only available when the user is signed into PSN.
			if (signedIn)
			{
#if UNITY_PSP2
				if (m_MenuMain.AddItem("Friends", signedIn))
				{
					m_MenuStack.PushMenu(m_Friends.GetMenu());
				}
#else
				if (m_MenuMain.AddItem("Friends & SNS", signedIn))
				{
					m_MenuStack.PushMenu(m_Friends.GetMenu());
				}
#endif
				if (m_MenuMain.AddItem("Ranking", signedIn))
				{
					m_MenuStack.PushMenu(m_Ranking.GetMenu());
				}
				if (m_MenuMain.AddItem("Matching", signedIn))
				{
					m_MenuStack.PushMenu(m_Sessions.GetMenu());
				}

				if (m_MenuMain.AddItem("Messaging", signedIn))
				{
					m_MenuStack.PushMenu(m_Messaging.GetMenu());
				}

				if (m_MenuMain.AddItem("Cloud Storage (TUS/TSS)", signedIn))
				{
					m_MenuStack.PushMenu(m_CloudStorage.GetMenu());
				}

				if (m_MenuMain.AddItem("Commerce", signedIn))
				{
					m_MenuStack.PushMenu(m_Commerce.GetMenu());
				}

	#if UNITY_PS4
				if (m_MenuMain.AddItem("Requests", signedIn))
				{
					m_MenuStack.PushMenu(m_Requests.GetMenu());
				}		
	#endif
			}

		}
	}

	public void OnEnter()
	{
	}

	public void OnExit()
	{
	}

	public void Process(MenuStack stack)
	{
		MenuMain();
	}

	void OnGUI()
	{
		MenuLayout activeMenu = m_MenuStack.GetMenu();
		activeMenu.GetOwner().Process(m_MenuStack);

		//// Add button to clear the on-screen log.
		//GUIStyle buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
		//buttonStyle.fontSize = 34 + 8;
		//buttonStyle.alignment = TextAnchor.MiddleCenter;
		//if (GUI.Button(new Rect(936 - 130, 350, 130, buttonStyle.fontSize + 16), "Clear", buttonStyle))
		//{
		//	OnScreenLog.Clear();
		//}
	}

	void OnLog(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.Text);
	}

	void OnLogWarning(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Warning: " + msg.Text);
	}

	void OnLogError(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Error: " + msg.Text);
	}

	void OnSignedIn(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.ToString());

		// Determine whether or not the Vita is in flight mode, i.e. signed in but no network connection.
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.User.GetLastSignInError(out result);
		if (result.lastError == Sony.NP.ErrorCode.NP_SIGNED_IN_FLIGHT_MODE)
		{
			OnScreenLog.Add("INFO: Signed in but flight mode is on");
		}
		else if (result.lastError != Sony.NP.ErrorCode.NP_OK)
		{
			OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
		}
	}

	void OnSomeEvent(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.ToString());
	}

	void OnConnectionDown(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Connection Down");

		// Determining the reason for loss of connection...
		//
		// When connection is lost we can call Sony.NP.System.GetLastConnectionError() to obtain
		// the NetCtl error status and reason for loss of connection.
		//
		// ResultCode.lastError will be either NP_ERR_NOT_CONNECTED
		// or NP_ERR_NOT_CONNECTED_FLIGHT_MODE.
		//
		// For the case where ResultCode.lastError == NP_ERR_NOT_CONNECTED further information about
		// the disconnection reason can be inferred from ResultCode.lastErrorSCE which contains
		// the SCE NetCtl error code relating to the disconnection (please refer to SCE SDK docs when
		// interpreting this code).

		// Get the reason for loss of connection...
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.System.GetLastConnectionError(out result);
		OnScreenLog.Add("Reason: " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
	}

	void OnSignInError(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.User.GetLastSignInError(out result);
		OnScreenLog.Add(result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
	}

	IEnumerator DoJoinSessionFromInvite()
	{
		// Leave the current session.
		if (Sony.NP.Matching.InSession)
		{
			OnScreenLog.Add("Leaving current session...");
			Sony.NP.Matching.LeaveSession();

			// Wait for session exit.
			while (Sony.NP.Matching.SessionIsBusy)
			{
				yield return null;
			}
		}

	

		// In order for member attributes to work, we must initialise them to a value before joining from an invite
		OnScreenLog.Add("Setting invited member attributes...");
		Sony.NP.Matching.ClearSessionAttributes();

		Sony.NP.Matching.SessionAttribute attrib;
		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = "CAR_TYPE";
		attrib.binValue = "CATMOB";
		Sony.NP.Matching.AddSessionAttribute(attrib);
		
		 // Join the session we were invited to.
		OnScreenLog.Add("Joining invited session...");

		Sony.NP.Matching.SessionAttributeInfo passAttribute;
		if ( Sony.NP.Matching.GetSessionInviteSessionAttribute("PASSWORD", out passAttribute) == Sony.NP.ErrorCode.NP_OK)
		{
			OnScreenLog.Add("Found PASSWORD attribute ..." + passAttribute.attributeBinValue);
			if ( passAttribute.attributeBinValue == "YES" )
			{
				// we *HAVE* to pass the password in, as it isn't included in the invite message
				OnScreenLog.Add("Session requires password...");
				Sony.NP.Matching.JoinInvitedSession(m_Sessions.m_SessionPassword);	
			}
			else
			{
				OnScreenLog.Add("No password required...");
				Sony.NP.Matching.JoinInvitedSession();	
			}
		}
		else
		{
			// Just try to connect without a password
			Sony.NP.Matching.JoinInvitedSession();		
		}

		

		// Reset the menu stack and go to the session menu.
		m_MenuStack.SetMenu(m_MenuMain);
		m_MenuStack.PushMenu(m_Sessions.GetMenu());
	}

	// Received a session invite.
	void OnMessagingSessionInviteRetrieved(Sony.NP.Messages.PluginMessage msg)
	{
		StartCoroutine("DoJoinSessionFromInvite");
	}

		// Received a session invite.
	void OnMessagingSessionInviteReceived(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(" OnMessagingSessionInviteReceived " );
	}
   
	void OnMessagingSessionInviteAccepted(Sony.NP.Messages.PluginMessage msg)
	{

		OnScreenLog.Add(" OnMessagingSessionInviteAccepted " );
	}

	void OnPlayTogether(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(" OnPlayTogether ");

		Sony.NP.Messaging.PlayTogether playTogether;

		playTogether = Sony.NP.Messaging.GetPlayTogetherParams();

		if  (playTogether.hostUserId != -1 )
		{
			OnScreenLog.Add("    Host User = " + playTogether.hostUserId);
			OnScreenLog.Add("    Number Online Ids = " + playTogether.numberOnlineIds);

			for(int i = 0; i < playTogether.numberOnlineIds; i++)
			{
				OnScreenLog.Add("    Online Id = " + playTogether.GetOnlineId(i));
			}

			m_Sessions.StartOnPlayTogetherSession(playTogether);
		}
		else
		{
			OnScreenLog.Add(" Invalid Play Together data. ");
		}
	}

	

	//
	// Network server callbacks...
	//

	void OnServerInitialized(NetworkPlayer player)
	{
		OnScreenLog.Add("Server Initialized: " + player.ipAddress + ":" + player.port);
		//SpawnPlayer();
		OnScreenLog.Add(" Network.isServer: " + Network.isServer);
		OnScreenLog.Add(" Network.isClient: " + Network.isClient);
		OnScreenLog.Add(" Network.peerType: " + Network.peerType);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		OnScreenLog.Add("Player connected from " + player.ipAddress + ":" + player.port);
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		OnScreenLog.Add("Player disconnected " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}


	// Send some data using NetworkView.RPC
	void SendSessionData()
	{
		// Construct some data to attach to the message.
		SharedSessionData data = new SharedSessionData();
		data.id = m_SendCount++;
		data.text = "Here's some RPC data";
		data.item1 = 2;
		data.item2 = 987654321;
		byte[] bytes = data.WriteToBuffer();
		GetComponent<NetworkView>().RPC("RecieveSharedSessionData", RPCMode.Others, bytes);
	}

	// Receive some data using RPC.
	[RPC]
	void RecieveSharedSessionData(byte[] buffer)
	{
		SharedSessionData data = new SharedSessionData();
		data.ReadFromBuffer(buffer);
		OnScreenLog.Add("RPC Rec: id " + data.id + " - " + data.text + " item1: " + data.item1 + " item2: " + data.item2);
	}

	//
	// Network client callbacks...
	//

	void OnConnectedToServer()
	{
		OnScreenLog.Add("Connected to server...");
		OnScreenLog.Add(" Network.isServer: " + Network.isServer);
		OnScreenLog.Add(" Network.isClient: " + Network.isClient);
		OnScreenLog.Add(" Network.peerType: " + Network.peerType);
		//SpawnPlayer();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		OnScreenLog.Add("Disconnected from server " + info);
		m_Sessions.m_SendingData = false;
		m_SendCount = 0;
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		OnScreenLog.Add("Could not connect to server: " + error);
	}
}
