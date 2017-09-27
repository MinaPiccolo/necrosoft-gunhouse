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
	SonyNpTrophy m_Trophies;
	SonyNpUtilities m_Utilities;

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

	static public Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK) {
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

		// System events.
		Sony.NP.System.OnConnectionUp += OnSomeEvent;
		Sony.NP.System.OnConnectionDown += OnConnectionDown;
		Sony.NP.System.OnSysResume += OnSomeEvent;
		Sony.NP.System.OnSysNpMessageArrived += OnSomeEvent;
		Sony.NP.System.OnSysStorePurchase += OnSomeEvent;
		Sony.NP.System.OnSysStoreRedemption += OnSomeEvent;
		Sony.NP.System.OnSysEvent += OnSomeEvent;	// Some other event.

		// User events.
		Sony.NP.User.OnSignedIn += OnSignedIn;
		Sony.NP.User.OnSignedOut += OnSomeEvent;
		Sony.NP.User.OnSignInError += OnSignInError;

		m_User = new SonyNpUser();
		m_Trophies = new SonyNpTrophy();
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

		for (int i = 0; i < m_Avatars.Length; i++)
		{
			if (m_Avatars[i].pendingDownload)
			{
				m_Avatars[i].pendingDownload = false;
				StartCoroutine(DownloadAvatar(i));
			}
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
}
