using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices; 

public class SonyNpSession : IScreen
{
	MenuLayout m_MenuSession;
	MenuLayout m_MenuInSessionHosting;
	MenuLayout m_MenuInSessionClient;
	bool m_MatchingIsReady = false;
	int m_GameDetails = 100;
	int m_CarType = 0;
	int m_ServerPort = 25001;
	int m_ServerMaxConnections = 32;
	int m_AppVersion = 200;		// change this number so you only find games created with this script
#if UNITY_PS4
	bool m_EnableUDPP2P = true;
#elif UNITY_PSP2
	bool m_EnableUDPP2P = true;
#else
	bool m_EnableUDPP2P = false;
#endif

	// Flags for debug logging...
	bool m_LogSessionAttributes = true;
	bool m_LogSessionMemberIP = false;
	bool m_LogSessionMemberAttributes = true;

	// There is a known endian issue with the Sony npToolkit library when processing cross-play attributes.
	//
	// Although both PS4 and Vita are little-endian the Vita npToolkit library always assumes that it will be talking
	// to a PS3 which is big-endian and so swaps the byte order for the attribute name hash values, but PS4 npToolkit
	// assumes that hash values are little-endian.
	//
	// So, in order for the Vita and PS4 to recognize attributes from the other platforms the hashed attribute name must
	// be a byte palindrome, Sony use the Fowler–Noll–Vo hashing algorithm to hash attribute names and the resulting
	// 32bit bit pattern must be interchangeable on little and big endian systems, this means that the ASCII attribute
	// names must hash to a byte-palindrome, e.g. "WONKPxp" when hashed = 0x72, 0xfd, 0xfd, 0x72.
	//
	// With all of the above in mind - define an array of byte-palindromic names which we can then assign to attributes
	// as needed. Feel free to use these names in your own apps, if you need more then use the "AttributeNameGenerator"
	// command line tool supplied with this example project to generate as many names as needed.
	//
	// The "AttributeNameGenerator" tool can be found in UnityNpToolkit_Source.zip supplied in the Assets/Plugins/<platform>
	// folder, after un-zipping the source zip the pre-compiled executable and associated source are located in
	// "\UnityNpToolkit<platform>\AttributeNameGenerator"
	//
	static string[] sm_AttrPalindromes = new string[]
	{
		"ATTRHyS", // = 0xa8ebeba8
		"ATTRMAr", // = 0xa8cbcba8
		"ATTRUOs", // = 0x415e5e41
		"ATTSOsx", // = 0xaf4040af
		"ATTSYSo", // = 0xb0d2d2b0
		"ATTTAqS", // = 0xc95a5ac9
		"ATTTYjj", // = 0x290d0d29
		"ATTTdcy", // = 0x966e6e96
		"ATTTyGX", // = 0x964e4e96
		"ATTUowX", // = 0xd12121d1
		"ATTVYjN", // = 0xdb3535db
		"ATTVbHt", // = 0x8e98988e
		"ATTWACg", // = 0x9c93939c
		"ATTWIBZ", // = 0xd6c5c5d6
		"ATTWTKE", // = 0x69646469
		"ATTWiwd", // = 0x69848469
		"ATTXUZi", // = 0x000b0b00
		"ATTXqSr", // = 0x20cfcf20
		"ATTYmwo", // = 0x9a96969a
		"ATTZBCU", // = 0x1eeaea1e
		"ATTZGot", // = 0x1ecaca1e
		"ATTaCGp", // = 0xd94a4ad9
		"ATTaksu", // = 0xa63b3ba6
		"ATTanOT", // = 0xa61b1ba6
		"ATTbOVQ", // = 0x48cccc48
		"ATTbPBg", // = 0xbb3e3ebb
		"ATTbryO", // = 0x0ebaba0e
		"ATTbwQn", // = 0x0e9a9a0e
		"ATTcRHI", // = 0x10adad10
		"ATTceun", // = 0x496d6d49
		"ATTcxXK"  // = 0xe84848e8
	};

	// Assign byte-palindromic names to any attributes that we require..
	//
	// Note that we have attributes for "host platform ID" and "member platform ID", this is so that we
	// can detect when a session or member attribute was communicated from a different platform and then
	// do the appropriate endian swapping on the attributes value.
	//
	// External attributes.
	static string sm_AttrHostPlatformName = sm_AttrPalindromes[0];	// "HOST_PLATFORM"
	static string sm_AttrRaceTrackName = sm_AttrPalindromes[1];		// "RACE_TRACK"
	//
	// Internal attributes.
	static string sm_AttrGameDetailsName = sm_AttrPalindromes[2];	// "GAME_DETAILS"
	static string sm_AttrPasswordName = sm_AttrPalindromes[3];		// "PASSWORD"
	//
	// Member attributes.
	static string sm_AttrMemberPlatformName = sm_AttrPalindromes[4];	// "MEMBER_PLATFORM"
	static string sm_AttrCarClassName = sm_AttrPalindromes[5];			// "CAR_CLASS"
	static string sm_AttrCarTypeName = sm_AttrPalindromes[6];			// "CAR_TYPE"
	//
	// Search attributes.
	static string sm_AttrAppVersionName = sm_AttrPalindromes[7];	// "APP_VERSION"
	static string sm_AttrLevelName = sm_AttrPalindromes[8];			// "LEVEL"
	static string sm_AttrBinSearchName = sm_AttrPalindromes[9];		// "TEST_BIN_SEARCH"
	//
	// For convenience when debugging - create a dictionary mapping attribute names to human readable names.
	static Dictionary<string, string> sm_AttrNameLookup = new Dictionary<string, string> 
	{
		{ sm_AttrPalindromes[0], "HOST_PLATFORM" },
		{ sm_AttrPalindromes[1], "RACE_TRACK" },
		{ sm_AttrPalindromes[2], "GAME_DETAILS" },
		{ sm_AttrPalindromes[3], "PASSWORD" },
		{ sm_AttrPalindromes[4], "MEMBER_PLATFORM" }, 
		{ sm_AttrPalindromes[5], "CAR_CLASS" },
		{ sm_AttrPalindromes[6], "CAR_TYPE" },
		{ sm_AttrPalindromes[7], "APP_VERSION" },
		{ sm_AttrPalindromes[8], "LEVEL" },
		{ sm_AttrPalindromes[9], "TEST_BIN_SEARCH" },
	};

#if UNITY_PSP2
	int m_ThisPlatformID = 0;	// PS VIta.
#else
	int m_ThisPlatformID = 1;	// Assume PS4
#endif

	public string m_SessionPassword = "password";

	public bool m_SendingData = false;
	Sony.NP.Matching.Session[] m_AvailableSessions = null;
	Nullable<Sony.NP.Matching.SessionMemberInfo> m_Host = null;       // Session member who is the host.
	Nullable<Sony.NP.Matching.SessionMemberInfo> m_Myself = null;     // Session member who is me.
	Nullable<Sony.NP.Matching.SessionMemberInfo> m_Connected = null;  // Session member that I'm connected to, should = host.
	Sony.NP.Matching.FlagSessionCreate m_SignallingType = Sony.NP.Matching.FlagSessionCreate.CREATE_SIGNALING_MESH_SESSION;

	enum PlayTogetherState
	{
		None,
		ReceivedEvent,
		WaitingForSessionCreation,
		SendingInvitations,
		Done,
	}

	PlayTogetherState m_PlayTogetherState = PlayTogetherState.None;
	Sony.NP.Messaging.PlayTogether m_ptCurrentSettings; // Player together settings;


	// Class for holding attributes and other info for the current matching session.
	class SessionAttributes
	{
		public void Initialize(Sony.NP.Matching.SessionAttributeInfo[] attributes, int thisPlatformID)
		{
			// Get the host platform attribute, so we know if we need to endian-swap any attribute values.
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].attributeName == sm_AttrHostPlatformName)
				{
					m_PlatformID = (attributes[i].attributeIntValue != 0) ? 1 : 0;
					break;
				}
			}

			// Store the session attributes and endian-swap any values that need it.
			sm_Attributes = new Sony.NP.Matching.SessionAttributeInfo[attributes.Length];
			for (int i = 0; i < sm_Attributes.Length; i++)
			{
				sm_Attributes[i] = attributes[i];

				if (sm_Attributes[i].attributeValueType == Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_INT)
				{
					switch (sm_Attributes[i].attributeType)
					{
						case Sony.NP.Matching.EnumAttributeType.SESSION_EXTERNAL_ATTRIBUTE:
						case Sony.NP.Matching.EnumAttributeType.SESSION_INTERNAL_ATTRIBUTE:
							if (m_PlatformID != thisPlatformID)
							{
								sm_Attributes[i].attributeIntValue = (int)EndianSwap((uint)sm_Attributes[i].attributeIntValue);
							}
							break;
					}
				}
			}
		}

		public void Reset()
		{
			sm_Attributes = null;
		}

		public void LogAttributes()
		{
			OnScreenLog.Add("  Session Attributes");
			OnScreenLog.Add("   Host platform id " + m_PlatformID);
			for (int i = 0; i < sm_Attributes.Length; i++)
			{
				if (sm_Attributes[i].attributeName != null)
				{
					string attr = "   Attribute " + i + ": " + BuildDebugReadableAttributeName(sm_Attributes[i].attributeName);
					switch (sm_Attributes[i].attributeValueType)
					{
						case Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_INT:
							attr += " = " + sm_Attributes[i].attributeIntValue;
							break;

						case Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_BINARY:
							attr += " = " + sm_Attributes[i].attributeBinValue;
							break;

						default:
							attr += ", has bad value type";
							break;
					}
					attr += ", " + sm_Attributes[i].attributeType;
					OnScreenLog.Add(attr);
				}
			}
		}

		public int m_PlatformID;	// The session host's platform ID.
		public Sony.NP.Matching.SessionAttributeInfo[] sm_Attributes;
	}


	// Class for holding attributes and other info for each member in the session.
	class MemberAttributes
	{
		public void Initialize(Sony.NP.Matching.SessionAttributeInfo[] attributes, int thisPlatformID)
		{
			// Get the host platform attribute, so we know if we need to endian-swap any attribute values.
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].attributeName == sm_AttrMemberPlatformName)
				{
					m_PlatformID = (attributes[i].attributeIntValue != 0) ? 1 : 0;
					break;
				}
			}

			// Store the member attributes and endian-swap any values that need it.
			sm_Attributes = new Sony.NP.Matching.SessionAttributeInfo[attributes.Length];
			for (int i = 0; i < sm_Attributes.Length; i++)
			{
				sm_Attributes[i] = attributes[i];

				if (sm_Attributes[i].attributeValueType == Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_INT)
				{
					if (m_PlatformID != thisPlatformID)
					{
						sm_Attributes[i].attributeIntValue = (int)EndianSwap((uint)sm_Attributes[i].attributeIntValue);
					}
				}
			}
		}

		public void Reset()
		{
			sm_Attributes = null;
		}

		public void LogAttributes()
		{
			OnScreenLog.Add("   Member Attributes");
			OnScreenLog.Add("    Platform id " + m_PlatformID);

			if (sm_Attributes.Length == 0)
			{
				OnScreenLog.Add("    No Member Attributes");
			}

			for (int j = 0; j < sm_Attributes.Length; j++)
			{
				string attr = "    Attribute " + j + ": " + BuildDebugReadableAttributeName(sm_Attributes[j].attributeName);
				switch (sm_Attributes[j].attributeValueType)
				{
					case Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_INT:
						attr += " = " + sm_Attributes[j].attributeIntValue;
						break;

					case Sony.NP.Matching.EnumAttributeValueType.SESSION_ATTRIBUTE_VALUE_BINARY:
						attr += " = " + sm_Attributes[j].attributeBinValue;
						break;

					default:
						attr += ", has bad value type";
						break;
				}
				OnScreenLog.Add(attr);
			}
		}

		int m_PlatformID;	// The members platform ID.
		Sony.NP.Matching.SessionAttributeInfo[] sm_Attributes;
	}

	SessionAttributes m_SessionAttributes;
	MemberAttributes[] m_MemberAttributes;


	static string BuildDebugReadableAttributeName(string palidromicName)
	{
		if (sm_AttrNameLookup.ContainsKey(palidromicName))
		{
			return palidromicName + " (" + sm_AttrNameLookup[palidromicName] + ")";
		}
		return palidromicName;
	}

	static uint EndianSwap(uint v)
	{
		// swap adjacent 16-bit blocks
		v = (v >> 16) | (v << 16);
		// swap adjacent 8-bit blocks
		return ((v & 0xFF00FF00) >> 8) | ((v & 0x00FF00FF) << 8);
	}

	public SonyNpSession()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_MenuSession;
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
			Sony.NP.Matching.GetLastError(out result);
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
		MenuSession(stack);
	}
	
	public void Initialize()
	{
		m_MenuSession = new MenuLayout(this, 550, 34);
		m_MenuInSessionHosting = new MenuLayout(this, 450, 34);
		m_MenuInSessionClient = new MenuLayout(this, 450, 34);
		
		Sony.NP.Matching.OnCreatedSession += OnMatchingCreatedSession;
		Sony.NP.Matching.OnFoundSessions += OnMatchingFoundSessions;
		Sony.NP.Matching.OnJoinedSession += OnMatchingJoinedSession;
		Sony.NP.Matching.OnJoinInvalidSession += OnMatchingJoinInvalidSession;
		Sony.NP.Matching.OnUpdatedSession += OnMatchingUpdatedSession;
		Sony.NP.Matching.OnLeftSession += OnMatchingLeftSession;
		Sony.NP.Matching.OnSessionDestroyed += OnMatchingSessionDestroyed;
		Sony.NP.Matching.OnKickedOut += OnMatchingKickedOut;
		Sony.NP.Matching.OnSessionError += OnSessionError;

		// First Initialize the session attribute definitions.
		Sony.NP.Matching.ClearAttributeDefinitions();
		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrLevelName, Sony.NP.Matching.EnumAttributeType.SESSION_SEARCH_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrAppVersionName, Sony.NP.Matching.EnumAttributeType.SESSION_SEARCH_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionBin(sm_AttrBinSearchName, Sony.NP.Matching.EnumAttributeType.SESSION_SEARCH_ATTRIBUTE, Sony.NP.Matching.EnumAttributeMaxSize.SESSION_ATTRIBUTE_MAX_SIZE_60);

		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrHostPlatformName, Sony.NP.Matching.EnumAttributeType.SESSION_EXTERNAL_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionBin(sm_AttrRaceTrackName, Sony.NP.Matching.EnumAttributeType.SESSION_EXTERNAL_ATTRIBUTE, Sony.NP.Matching.EnumAttributeMaxSize.SESSION_ATTRIBUTE_MAX_SIZE_12);
	
		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrGameDetailsName, Sony.NP.Matching.EnumAttributeType.SESSION_INTERNAL_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionBin(sm_AttrPasswordName, Sony.NP.Matching.EnumAttributeType.SESSION_INTERNAL_ATTRIBUTE, Sony.NP.Matching.EnumAttributeMaxSize.SESSION_ATTRIBUTE_MAX_SIZE_12);

		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrMemberPlatformName, Sony.NP.Matching.EnumAttributeType.SESSION_MEMBER_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionInt(sm_AttrCarClassName, Sony.NP.Matching.EnumAttributeType.SESSION_MEMBER_ATTRIBUTE);
		Sony.NP.Matching.AddAttributeDefinitionBin(sm_AttrCarTypeName, Sony.NP.Matching.EnumAttributeType.SESSION_MEMBER_ATTRIBUTE, Sony.NP.Matching.EnumAttributeMaxSize.SESSION_ATTRIBUTE_MAX_SIZE_28);
		
		ErrorHandler(Sony.NP.Matching.RegisterAttributeDefinitions());	
	}

	public void MenuSession(MenuStack menuStack)
	{
		bool matchingAvailable = Sony.NP.User.IsSignedInPSN;
		bool inSession = Sony.NP.Matching.InSession;

		if (m_MatchingIsReady == false && matchingAvailable)
		{
			m_MatchingIsReady = true;
		}

		if (inSession)
		{
			MenuInSession(menuStack);
		}
		else
		{
			MenuSetupSession(menuStack);
		}
	}

	// Setup the session attributes that a session will be created with.
	void SetupNewSessionAttributes()
	{
		Sony.NP.Matching.ClearSessionAttributes();

		Sony.NP.Matching.SessionAttribute attrib;
		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrAppVersionName;
		attrib.intValue = m_AppVersion;
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrBinSearchName;
		attrib.binValue = "BIN_VALUE";
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrLevelName;
		attrib.intValue = 1;
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrPasswordName;
		attrib.binValue = "NO";
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrMemberPlatformName;
		attrib.intValue = m_ThisPlatformID;
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrCarClassName;
		attrib.intValue = 5;
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrCarTypeName;
		attrib.binValue = "CATMOB";
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrHostPlatformName;
		attrib.intValue = m_ThisPlatformID;
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrRaceTrackName;
		attrib.binValue = "TURKEY";
		Sony.NP.Matching.AddSessionAttribute(attrib);

		attrib = new Sony.NP.Matching.SessionAttribute();
		attrib.name = sm_AttrGameDetailsName;
		attrib.intValue = m_GameDetails;
		Sony.NP.Matching.AddSessionAttribute(attrib);
	}

	public void MenuSetupSession(MenuStack menuStack)
	{
		bool matchingAvailable = Sony.NP.User.IsSignedInPSN;
		bool inSession = Sony.NP.Matching.InSession;
		bool sessionBusy = Sony.NP.Matching.SessionIsBusy;

		m_MenuSession.Update();

		bool foundSessions = (m_AvailableSessions != null) && (m_AvailableSessions.Length > 0);

		if (foundSessions)
		{
			// We would normally present a list of found sessions to the user and let them select which one to join but for the sake of a simple example
			// lets just give the option to join the first one.
			if (m_MenuSession.AddItem("Join 1st Found Session", matchingAvailable && foundSessions && !inSession && !sessionBusy))
			{
				OnScreenLog.Add("Joining PSN session: " + m_AvailableSessions[0].sessionInfo.sessionName);

				// First setup the session member attributes.
				Sony.NP.Matching.ClearSessionAttributes();

				Sony.NP.Matching.SessionAttribute attrib;

				attrib = new Sony.NP.Matching.SessionAttribute();
				attrib.name = sm_AttrMemberPlatformName;
				attrib.intValue = m_ThisPlatformID;
				Sony.NP.Matching.AddSessionAttribute(attrib);

				attrib = new Sony.NP.Matching.SessionAttribute();
				attrib.name = sm_AttrCarClassName;
				attrib.intValue = 5;
				Sony.NP.Matching.AddSessionAttribute(attrib);

				attrib = new Sony.NP.Matching.SessionAttribute();
				attrib.name = sm_AttrCarTypeName;
				attrib.binValue = "CATMOB";
				Sony.NP.Matching.AddSessionAttribute(attrib);

				ErrorHandler(Sony.NP.Matching.JoinSession(m_AvailableSessions[0].sessionInfo.sessionID, m_SessionPassword));

				m_AvailableSessions = null;
			}
		}
		else
		{
			if (m_MenuSession.AddItem("Create & Join Session", matchingAvailable && !inSession && !sessionBusy))
			{
				OnScreenLog.Add("Creating session...");

				// First setup the session attributes that the session will be created with.
				SetupNewSessionAttributes();

				string name = "Test Session";
				int serverID = 0;
				int worldID = 0;
				int numSlots = 8;
				string password = "";
				string sessionStatus = "Toolkit Sample Session";	// Only used on PS4 and PSP2.

				ErrorHandler(Sony.NP.Matching.CreateSession(name, serverID, worldID, numSlots, password,
											m_SignallingType,		// creation flags
											Sony.NP.Matching.EnumSessionType.SESSION_TYPE_PUBLIC,	// type flags
											sessionStatus));
			}

			// Private sessions MUST have passwords. If using with invites, the password must be known by the person invited, it isn't passed with the invite
			if (m_MenuSession.AddItem("Create & Join Private Session", matchingAvailable && !inSession && !sessionBusy))
			{
				OnScreenLog.Add("Creating private session... password is required");

				// First setup the session attributes that the session will be created with.
				SetupNewSessionAttributes();

				// Create the session.
				string name = "Test Session";
				int serverID = 0;
				int worldID = 0;
				int numSlots = 8;
				string password = m_SessionPassword;
				string sessionStatus = "Toolkit Sample Session";	// Only used on PS4 and PSP2.

				ErrorHandler(Sony.NP.Matching.CreateSession(name, serverID, worldID, numSlots, password,
											m_SignallingType | Sony.NP.Matching.FlagSessionCreate.CREATE_PASSWORD_SESSION,  // creation flags
											Sony.NP.Matching.EnumSessionType.SESSION_TYPE_PRIVATE,	// type flags
											sessionStatus));
			}

			// friend sessions also must have passwords
			if (m_MenuSession.AddItem("Create & Join Friend Session", matchingAvailable && !inSession && !sessionBusy))
			{
				OnScreenLog.Add("Creating Friend session...");

				// First setup the session attributes that the session will be created with.
				SetupNewSessionAttributes();

				// Create the session.
				string name = "Test Session";
				int serverID = 0;
				int worldID = 0;
				int numSlots = 8;
				int numFriendslots = 8;		// all slots for friends
				string password = m_SessionPassword;
				string sessionStatus = "Toolkit Sample Session";	// Only used on PS4 and PSP2.

				// A friend session uses 0 as the sessionTypeFlag, which means we are required to define the slot information
				ErrorHandler(Sony.NP.Matching.CreateFriendsSession(name, serverID, worldID, numSlots, numFriendslots, password,
											m_SignallingType | Sony.NP.Matching.FlagSessionCreate.CREATE_PASSWORD_SESSION,  // creation flags
											sessionStatus));
			}		

			if (m_MenuSession.AddItem("Find Sessions", matchingAvailable && !inSession && !sessionBusy))
		    {
			    OnScreenLog.Add("Finding sessions...");

			    // First setup the session attributes to use for the search.
			    Sony.NP.Matching.ClearSessionAttributes();

			    Sony.NP.Matching.SessionAttribute attrib;
			    attrib = new Sony.NP.Matching.SessionAttribute();
			    attrib.name = sm_AttrAppVersionName;
			    attrib.intValue = m_AppVersion;
			    attrib.searchOperator = Sony.NP.Matching.EnumSearchOperators.MATCHING_OPERATOR_EQ;
			    Sony.NP.Matching.AddSessionAttribute(attrib);
			
			    int serverID = 0;
			    int worldID = 0;

			    // Start searching.
			    ErrorHandler(Sony.NP.Matching.FindSession(serverID, worldID));
		    }
		    if (m_MenuSession.AddItem("Find Sessions (bin search)", matchingAvailable && !inSession && !sessionBusy))
		    {
			    OnScreenLog.Add("Finding sessions...");

			    // First setup the session attributes to use for the search.
			    Sony.NP.Matching.ClearSessionAttributes();
		
			    Sony.NP.Matching.SessionAttribute attrib;
			    attrib = new Sony.NP.Matching.SessionAttribute();
			    attrib.name = sm_AttrBinSearchName;
			    attrib.binValue = "BIN_VALUE";
			    attrib.searchOperator = Sony.NP.Matching.EnumSearchOperators.MATCHING_OPERATOR_EQ;
			    Sony.NP.Matching.AddSessionAttribute(attrib);
		
			    int serverID = 0;
			    int worldID = 0;

			    // Start searching for binary ... use regional session flag as workaround to make binary search work (also use EnumAttributeMaxSize.SESSION_ATTRIBUTE_MAX_SIZE_60 in RegisterAttributeDefinitions() )
			    // see https://ps4.scedev.net/support/issue/60812
			    ErrorHandler(Sony.NP.Matching.FindSession(serverID, worldID, Sony.NP.Matching.FlagSessionSearch.SEARCH_REGIONAL_SESSIONS));
		    }

		    if (m_MenuSession.AddItem("Find Friend Sessions", matchingAvailable && !inSession && !sessionBusy))
		    {
			    OnScreenLog.Add("Finding friend sessions...");

			    // First setup the session attributes to use for the search.
			    Sony.NP.Matching.ClearSessionAttributes();

			    Sony.NP.Matching.SessionAttribute attrib;
			    attrib = new Sony.NP.Matching.SessionAttribute();
			    attrib.name = sm_AttrAppVersionName;
			    attrib.intValue = m_AppVersion;
			    attrib.searchOperator = Sony.NP.Matching.EnumSearchOperators.MATCHING_OPERATOR_EQ;
			    Sony.NP.Matching.AddSessionAttribute(attrib);

			    int serverID = 0;
			    int worldID = 0;

			    // Start searching.
			    ErrorHandler(Sony.NP.Matching.FindSession(serverID, worldID, Sony.NP.Matching.FlagSessionSearch.SEARCH_FRIENDS_SESSIONS));
		    }

		    if (m_MenuSession.AddItem("Find Regional Sessions", matchingAvailable && !inSession && !sessionBusy))
		    {
			    OnScreenLog.Add("Finding friend sessions...");

			    // First setup the session attributes to use for the search.
			    Sony.NP.Matching.ClearSessionAttributes();

			    Sony.NP.Matching.SessionAttribute attrib;
			    attrib = new Sony.NP.Matching.SessionAttribute();
			    attrib.name = sm_AttrAppVersionName;
			    attrib.intValue = m_AppVersion;
			    attrib.searchOperator = Sony.NP.Matching.EnumSearchOperators.MATCHING_OPERATOR_EQ;
			    Sony.NP.Matching.AddSessionAttribute(attrib);

			    int serverID = 0;
			    int worldID = 0;

			    // Start searching.
			    ErrorHandler(Sony.NP.Matching.FindSession(serverID, worldID, Sony.NP.Matching.FlagSessionSearch.SEARCH_REGIONAL_SESSIONS));
		    }

		    /*
			    Any of the FlagSessionSearch flags can be combined.
			
				    SEARCH_FRIENDS_SESSIONS = (1 << 10),        // This flag specifies that the search is for a friend’s session.
				    SEARCH_REGIONAL_SESSIONS = (1 << 12),	    // This flag specifies that the search is for a session that is hosted in your region.
				    SEARCH_RECENTLY_MET_SESSIONS = (1 << 14),	// This flag specifies that the search is for a session hosted by users in the Recently Met List.
				    SEARCH_RANDOM_SESSIONS = (1 << 18),	        //This flag specifies that the search is for a session with whom a P2P session can be established.
				    SEARCH_NAT_RESTRICTED_SESSIONS = (1 << 20),	// This flag specifies that users who cannot establish P2P connections are not allowed to join the session. 
			
		    */
		    if (m_MenuSession.AddItem("Find Random Sessions", matchingAvailable && !inSession && !sessionBusy))
		    {
			    OnScreenLog.Add("Finding sessions in a random order...");

			    // First setup the session attributes to use for the search.
			    Sony.NP.Matching.ClearSessionAttributes();

			    Sony.NP.Matching.SessionAttribute attrib;
			    attrib = new Sony.NP.Matching.SessionAttribute();
			    attrib.name = sm_AttrAppVersionName;
			    attrib.intValue = m_AppVersion;
			    attrib.searchOperator = Sony.NP.Matching.EnumSearchOperators.MATCHING_OPERATOR_EQ;
			    Sony.NP.Matching.AddSessionAttribute(attrib);

			    int serverID = 0;
			    int worldID = 0;

			    // Start searching.
			    ErrorHandler(Sony.NP.Matching.FindSession(serverID, worldID, Sony.NP.Matching.FlagSessionSearch.SEARCH_RANDOM_SESSIONS));
		    }		
		}
		

		if (m_MenuSession.AddBackIndex("Back"))
		{
			menuStack.PopMenu();
		}
	}

	public void MenuInSession(MenuStack menuStack)
	{
		bool matchingAvailable = Sony.NP.User.IsSignedInPSN;
		bool inSession = Sony.NP.Matching.InSession;
		bool sessionBusy = Sony.NP.Matching.SessionIsBusy;
		bool isHosting = Sony.NP.Matching.IsHost;

		MenuLayout layout = isHosting ? m_MenuInSessionHosting : m_MenuInSessionClient;
		layout.Update();

		if (isHosting)
		{
			if (layout.AddItem("Modify Session", matchingAvailable && inSession && !sessionBusy))
			{
				OnScreenLog.Add("Modifying session...");

				// First setup the session attributes to modify.
				Sony.NP.Matching.ClearModifySessionAttributes();

				m_GameDetails += 100;

				Sony.NP.Matching.ModifySessionAttribute attrib;
				attrib = new Sony.NP.Matching.ModifySessionAttribute();
				attrib.name = sm_AttrGameDetailsName;
				attrib.intValue = m_GameDetails;
				Sony.NP.Matching.AddModifySessionAttribute(attrib);

				ErrorHandler(Sony.NP.Matching.ModifySession(Sony.NP.Matching.EnumAttributeType.SESSION_INTERNAL_ATTRIBUTE));
			}
		}

		if (layout.AddItem("Modify Member Attribute", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Modifying Member Attribute...");

			// First setup the session attributes to modify.
			Sony.NP.Matching.ClearModifySessionAttributes();

			Sony.NP.Matching.ModifySessionAttribute attrib;
			attrib = new Sony.NP.Matching.ModifySessionAttribute();
			attrib.name = sm_AttrCarTypeName;
			m_CarType++;
			if (m_CarType>3) m_CarType=0;
			switch(m_CarType)
			{
				case 0:
				attrib.binValue = "CATMOB";			
				break;
				case 1:
				attrib.binValue = "CARTYPE1";			
				break;
				case 2:
				attrib.binValue = "CARTYPE2";			
				break;
				case 3:
				attrib.binValue = "CARTYPE3";			
				break;
			}
			
			attrib.intValue = m_GameDetails;
			Sony.NP.Matching.AddModifySessionAttribute(attrib);

			ErrorHandler(Sony.NP.Matching.ModifySession(Sony.NP.Matching.EnumAttributeType.SESSION_MEMBER_ATTRIBUTE));
		}

		if (m_SendingData == false)
		{
			// Start sending shared session data via NetworkView.RPC
			if (layout.AddItem("Start Sending Data", matchingAvailable && inSession && !sessionBusy))
			{
				m_SendingData = true;
			}
		}
		else
		{
			// Start sending shared session data via NetworkView.RPC
			if (layout.AddItem("Stop Sending Data", matchingAvailable && inSession && !sessionBusy))
			{
				m_SendingData = false;
			}
		}

		if (layout.AddItem("Leave Session", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Leaving session...");
			LeaveSession();
		}

		if (layout.AddItem("List session members", matchingAvailable && inSession && !sessionBusy))
		{
			Sony.NP.Matching.Session session = Sony.NP.Matching.GetSession();
			Sony.NP.Matching.SessionMemberInfo[] members = session.members;
			for (int i = 0; i < members.Length; i++)
			{
				Sony.NP.Matching.SessionMemberInfo member = members[i];
				string log = i + "/memberId:" + member.memberId 
					+ "/memberFlag:" + member.memberFlag 
					+ "/addr:" + member.addr 
					+ "/natType:" + member.natType
					+ "/port:" + member.port;
				OnScreenLog.Add(log);
			}
		}
		
		if (layout.AddItem("Invite Friend", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Invite Friend...");
			ErrorHandler(Sony.NP.Matching.InviteToSession("Invite Test", 8));
		}
		
		if (layout.AddItem("Edit & Invite NpIds", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Invite NpIds...");
			string [] InvitedOnlineIDs = new string[] {"ergtjc", "Q-ZLqkCtBK-GB-EN"};
			bool userEditable = true;		// set to true if you want the user to have the chance to edit the list in a dialog
			ErrorHandler(Sony.NP.Matching.InviteToSession("Invite Test", InvitedOnlineIDs, userEditable));
		}

		if (layout.AddItem("Invite NpIds", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Invite NpIds...");
			string [] InvitedOnlineIDs = new string[] {"ergtjc", "Q-ZLqkCtBK-GB-EN"};
			bool userEditable = false;		// set to true if you want the user to have the chance to edit the list in a dialog
			ErrorHandler(Sony.NP.Matching.InviteToSession("Invite Test", InvitedOnlineIDs, userEditable));
		}

		if (layout.AddItem("Kick Member", matchingAvailable && inSession && !sessionBusy))
		{
			OnScreenLog.Add("Kick Member...");

			Sony.NP.Matching.Session session = Sony.NP.Matching.GetSession();
			Sony.NP.Matching.SessionMemberInfo[] members = session.members;

			if (members.Length > 1)
			{
				ErrorHandler(Sony.NP.Matching.KickMember(members[1].memberId, true));
			}
		}		
		
		if (layout.AddBackIndex("Back"))
		{
			menuStack.PopMenu();
		}

		if (Sony.NP.Matching.IsHost)
		{
			NetworkPlayer[] players = Network.connections;
			GUI.Label(new Rect(Screen.width - 200, Screen.height - 200, 200, 64), players.Length.ToString());
		}
	}

	// Find the host in the sessions member list.
	Nullable<Sony.NP.Matching.SessionMemberInfo> FindHostMember(Sony.NP.Matching.Session session)
	{
		Sony.NP.Matching.SessionMemberInfo[] members = session.members;

		for (int i = 0; i < members.Length; i++)
		{
			if((members[i].memberFlag & Sony.NP.Matching.FlagMemberType.MEMBER_OWNER) != 0)
			{
				return members[i];
			}
		}

		return null;
	}

	// Find myself in the sessions member list.
	Nullable<Sony.NP.Matching.SessionMemberInfo> FindSelfMember(Sony.NP.Matching.Session session)
	{
		Sony.NP.Matching.SessionMemberInfo[] members = session.members;

		for (int i = 0; i < members.Length; i++)
		{
			if ((members[i].memberFlag & Sony.NP.Matching.FlagMemberType.MEMBER_MYSELF) != 0)
			{
				return members[i];
			}
		}

		return null;
	}

	// Store member info for the host and client (myself).
	bool InitializeHostAndSelf(Sony.NP.Matching.Session session)
	{
		m_Host = FindHostMember(session);
		if (m_Host == null)
		{
			OnScreenLog.Add("Host member not found!");
			return false;
		}

		m_Myself = FindSelfMember(session);
		if (m_Myself == null)
		{
			OnScreenLog.Add("Self member not found!");
			return false;
		}

		return true;
	}

	// OnFoundSessions event handler; called when a session search has completed.
	void OnMatchingFoundSessions(Sony.NP.Messages.PluginMessage msg)
	{
		Sony.NP.Matching.Session[] sessions = Sony.NP.Matching.GetFoundSessionList();

		OnScreenLog.Add("Found " + sessions.Length + " sessions");
		for (int i = 0; i < sessions.Length; i++)
		{
			m_SessionAttributes = new SessionAttributes();
			m_SessionAttributes.Initialize(sessions[i].sessionAttributes, m_ThisPlatformID);
			LogSessionInfo(sessions[i]);
		}

		m_AvailableSessions = sessions;
	}

	string IntIPToIPString(int ip)
	{
		int a = ip & 255;
		int b = (ip >> 8) & 255;
		int c = (ip >> 16) & 255;
		int d = (ip >> 24) & 255;
		string sip = a.ToString() + "." + b.ToString() + "." + c.ToString() + "." + d.ToString();
		return sip;
	}

	void LeaveSession()
	{
		ErrorHandler(Sony.NP.Matching.LeaveSession());
	}

	void LogSessionInfo(Sony.NP.Matching.Session session)
	{
		Sony.NP.Matching.SessionInfo info = session.sessionInfo;
		Sony.NP.Matching.SessionAttributeInfo[] attributes = session.sessionAttributes;
		Sony.NP.Matching.SessionMemberInfo[] members = session.members;

		OnScreenLog.Add(" Session: " + info.sessionName);
		OnScreenLog.Add("  Members " + info.numMembers + ", max members " + info.maxMembers);
		OnScreenLog.Add("  Open slots " + info.openSlots + ", reserved slots " + info.reservedSlots);
		OnScreenLog.Add("  World id " + info.worldId.ToString("X") + ", room id " + info.roomId.ToString("X"));
		OnScreenLog.Add("  This platform id " + m_ThisPlatformID);

		if (m_LogSessionAttributes && m_SessionAttributes != null)
		{
			m_SessionAttributes.LogAttributes();
		}

		if (members == null)
		{
			return;
		}

		for (int i = 0; i < members.Length; i++)
		{
			OnScreenLog.Add("  Member " + i + ": " + members[i].npOnlineID + ", " +"Type: " + members[i].memberFlag);
			if (m_LogSessionMemberIP)
			{
				if (members[i].addr != 0)
				{
					OnScreenLog.Add("   IP: " + IntIPToIPString(members[i].addr) + " port " + members[i].port + " 0x" + members[i].port.ToString("X") );
				}
				else
				{
					OnScreenLog.Add("   IP: unknown " );
				}
			}

			if (m_LogSessionMemberAttributes && m_MemberAttributes != null)
			{
				m_MemberAttributes[i].LogAttributes();
			}
		}
	}

	// OnCreatedSession event handler; called when a session has been created.
	void OnMatchingCreatedSession(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Created session, enableUDPP2P = " + m_EnableUDPP2P);
		Sony.NP.Matching.Session session = Sony.NP.Matching.GetSession();

		m_SessionAttributes = new SessionAttributes();
		m_SessionAttributes.Initialize(session.sessionAttributes, m_ThisPlatformID);
	
		LogSessionInfo(session);

		if (InitializeHostAndSelf(session) == false)
		{
			OnScreenLog.Add("Error: Expected members not found!");
		}

		// Initialize the server.
#if UNITY_PS4
		UnityEngine.PS4.Networking.enableUDPP2P = m_EnableUDPP2P;
#elif UNITY_PSP2
		UnityEngine.PSVita.Networking.enableUDPP2P = m_EnableUDPP2P;
#endif
		NetworkConnectionError err = Network.InitializeServer(m_ServerMaxConnections, m_ServerPort, false);
		if (err != NetworkConnectionError.NoError)
		{
			OnScreenLog.Add("Server err: " + err);
		}
		else
		{
			OnScreenLog.Add("Started Server");
		}

		if (m_PlayTogetherState == PlayTogetherState.WaitingForSessionCreation)
		{
			OnPlayTogetherInvites(session);
		}
	}

	// OnJoinedSession event handler; called when a session has been successfully joined.
	void OnMatchingJoinedSession(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Joined PSN matching session... waiting on session info in OnMatchingUpdatedSession()");
		// Note session member info is not complete (missing connection data) until the system calls OnMatchingUpdatedSession()
		// so we'll defer setting up the network connection and do it in OnMatchingUpdatedSession().
	}

	// OnUpdatedSession event handler; called when the current sessions data has been modified.
	void OnMatchingUpdatedSession(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Session info updated...");
		Sony.NP.Matching.Session session = Sony.NP.Matching.GetSession();

		// Initialize session attributes.
		m_SessionAttributes = new SessionAttributes();
		m_SessionAttributes.Initialize(session.sessionAttributes, m_ThisPlatformID);

		// Initialize member attributes.
		m_MemberAttributes = new MemberAttributes[session.memberAttributes.Count];
		for (int i=0; i < session.memberAttributes.Count; i++)
		{
			m_MemberAttributes[i] = new MemberAttributes();
			m_MemberAttributes[i].Initialize(session.memberAttributes[i], m_ThisPlatformID);
		}

		LogSessionInfo(session);

		if (InitializeHostAndSelf(session) == false)
		{
			OnScreenLog.Add("Error: Expected members not found!");
			LeaveSession();
			return;
		}

		// If not already connected do it now.
		if ((Sony.NP.Matching.IsHost == false) && (m_Connected == null))
		{
			// Connect to the host.
			
			if (m_Host.Value.addr == 0)
			{
				// This is OK, we may receive multiple session update events while connecting.
				return;
			}

			// We have the host ID so we can connect now...

			OnScreenLog.Add("Retrieved host IP address");
			
			string hostIP = IntIPToIPString(m_Host.Value.addr);

			// If required pass the port value from the host to the networking code.
			// Port number actually opened by the recipient as seen from the internet.
			// The port number actually opened by the recipient as seen from the internet can be obtained
			// using the NpMatching library (refer to the "NpMatching2 System Overview" document), for example
#if UNITY_PS4
			OnScreenLog.Add("Set enableUDPP2P = " + m_EnableUDPP2P);

			UnityEngine.PS4.Networking.enableUDPP2P = m_EnableUDPP2P;
			if (m_EnableUDPP2P)
			{
				UnityEngine.PS4.Networking.NpSignalingPort = m_Host.Value.port;

				OnScreenLog.Add("Set NpSignalingPort = " + UnityEngine.PS4.Networking.NpSignalingPort);
			}
#elif  UNITY_PSP2
			OnScreenLog.Add("Set enableUDPP2P = " + m_EnableUDPP2P);

			UnityEngine.PSVita.Networking.enableUDPP2P = m_EnableUDPP2P;
			if (m_EnableUDPP2P)
			{
				UnityEngine.PSVita.Networking.npSignalingPort = m_Host.Value.port;
				
				OnScreenLog.Add("Set NpSignalingPort = " + UnityEngine.PSVita.Networking.npSignalingPort);
			}
#endif
		
			OnScreenLog.Add("Connecting to " + hostIP + ":" + m_ServerPort + " using signalling port:" + m_Host.Value.port);
			
			NetworkConnectionError err = Network.Connect(hostIP, m_ServerPort);		// important to pass the server port in
			if (err != NetworkConnectionError.NoError)
			{
				OnScreenLog.Add("Connection failed: " + err);
			}
			else
			{
				OnScreenLog.Add("Requesting server connection " + hostIP + " : " + m_ServerPort);
				m_Connected = m_Host;
			}
		}
	}

	// OnJoinInvalidSession event handler; called if an attempt to join a session failed.
	void OnMatchingJoinInvalidSession(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Failed to join session...");
		OnScreenLog.Add(" Session search results may be stale.");
	}

	// Set various session properties to default state, called after leaving a session.
	void ClearSessionProperties()
	{
		m_Host = null;
		m_Connected = null;
		m_Myself = null;
		m_SessionAttributes = null;
		m_MemberAttributes = null;
		m_AvailableSessions = null;
	}

	// OnLeftSession event handler; called when the player has left the current session.
	void OnMatchingLeftSession(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Left the session");
		
		Network.Disconnect(1);
		ClearSessionProperties();
	}

	// OnSessionDestroyed even handler; called when the current session has been destroyed.
	void OnMatchingSessionDestroyed(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Session Destroyed");
		
		Network.Disconnect(1);
		ClearSessionProperties();
	}

	// OnKickedOut event handler; called if the player has been kicked out of the current session.
	void OnMatchingKickedOut(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Kicked out of session");
		
		Network.Disconnect(1);
		ClearSessionProperties();
	}

	void OnSessionError(Sony.NP.Messages.PluginMessage msg)
	{
		ErrorHandler();
	}

	public void Update()
	{
		if (m_PlayTogetherState == PlayTogetherState.ReceivedEvent)
		{
			UpdatePlayTogetherSession();
		}
	}

	public void StartOnPlayTogetherSession(Sony.NP.Messaging.PlayTogether ptSettings)
	{
		m_ptCurrentSettings = ptSettings; // Player together settings;
		m_PlayTogetherState = PlayTogetherState.ReceivedEvent;
	}

	public void UpdatePlayTogetherSession()
	{
		// Test if the APP is in a state where the Play Together can be created.
		// APP must have reached a certain point during Initialisation, otherwise session
		// creation won't be possible.

		if (Sony.NP.User.IsSignedInPSN == false) return;
		if (Sony.NP.Trophies.TrophiesAreAvailable == false) return;

		OnScreenLog.Add(" Play Together - Creating Session");

		m_PlayTogetherState = PlayTogetherState.WaitingForSessionCreation;

		// Set host id that started the 'Play Together' session
		Sony.NP.User.SetCurrentUserId(m_ptCurrentSettings.hostUserId);

		// First setup the session attributes that the session will be created with.
		SetupNewSessionAttributes();

		string name = "PT Session";
		int serverID = 0;
		int worldID = 0;
		int numSlots = 8;
		string password = "";
		string sessionStatus = "Toolkit Sample Session";    // Only used on PS4 and PSP2.

		ErrorHandler(Sony.NP.Matching.CreateSession(name, serverID, worldID, numSlots, password,
									m_SignallingType,       // creation flags
									Sony.NP.Matching.EnumSessionType.SESSION_TYPE_PUBLIC,   // type flags
									sessionStatus));
	}

	void OnPlayTogetherInvites(Sony.NP.Matching.Session session)
	{
		m_PlayTogetherState = PlayTogetherState.SendingInvitations;

		OnScreenLog.Add(" Play Together - Sending out invitatations.");
		string[] InvitedOnlineIDs = new string[m_ptCurrentSettings.numberOnlineIds];

		for (int i = 0; i < m_ptCurrentSettings.numberOnlineIds; i++)
		{
			InvitedOnlineIDs[i] = m_ptCurrentSettings.GetOnlineId(i);
		}

		ErrorHandler(Sony.NP.Matching.InviteToSessionSilent("Play Together Invites", InvitedOnlineIDs));

		m_PlayTogetherState = PlayTogetherState.Done;
	}
}
