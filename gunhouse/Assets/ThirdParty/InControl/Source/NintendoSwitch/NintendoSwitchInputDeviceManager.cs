// NOTICE:
//
// This code contains proprietary information and is protected by national and
// international copyright laws. It may not be disclosed to third parties or
// copied or duplicated in any form, in whole or in part.
//
// The content herein is highly confidential and should be handled accordingly.
//
namespace InControl
{
	using nn.hid;


	// @cond nodoc
	public class NintendoSwitchInputDeviceManager : InputDeviceManager
	{
		/// <summary>
		/// Whether to enable six-axis sensors on devices by default.
		/// Don't turn this on unless you need it. It could take up unnecessary
		/// bandwidth and processing and/or limit the number of supported devices.
		/// </summary>
		internal const bool EnableSixAxisSensors = false;


		public NintendoSwitchInputDeviceManager()
		{
			DebugPad.Initialize();
			CreateDebugDevice();

			Npad.Initialize();

			// NpadJoy.SetHoldType only affects how controllers behave when using a system applet.
			// NpadJoyHoldType.Vertical is the default setting, but most games would want to use
			// NpadJoyHoldType.Horizontal as it can support all controller styles.
			//
			// If you do use NpadJoyHoldType.Vertical, Npad.SetSupportedStyleSet must only list
			// controller types supported by NpadJoyHoldType.Vertical. If you don't, the controller
			// applet will fail to load.
			//
			// Supported types for NpadJoyHoldType.Vertical are:
			// NpadStyle.FullKey, NpadStyle.JoyLeft and NpadStyle.JoyRight
			NpadJoy.SetHoldType( NpadJoyHoldType.Horizontal );

			// Handheld = Joy-Con docked with the console.
			// FullKey  = Pro Controller.
			// JoyDual  = Two Joy-Con used together as one controller.
			// JoyLeft  = Left Joy-Con used by itself.
			// JoyRight = Left Joy-Con used by itself.
			Npad.SetSupportedStyleSet( NpadStyle.Handheld | NpadStyle.FullKey | NpadStyle.JoyDual | NpadStyle.JoyLeft | NpadStyle.JoyRight );

			// Both controllers must be docked for handheld mode.
			NpadJoy.SetHandheldActivationMode( NpadHandheldActivationMode.Dual );

			// You must call Npad.SetSupportedIdType for all supported controllers.
			// Not calling this may lead to crashes in some circumstances.
			var nPadIds = new NpadId[] {
				NpadId.Handheld,
				NpadId.No1,
				NpadId.No2,
				NpadId.No3,
				NpadId.No4,
				NpadId.No5,
				NpadId.No6,
				NpadId.No7,
				NpadId.No8
			};
			Npad.SetSupportedIdType( nPadIds );

			// Create InControl devices for all NpadIds regardless of how many we request from NintendoSDK.
			CreateDevice( NpadId.Handheld );
			CreateDevice( NpadId.No1 );
			CreateDevice( NpadId.No2 );
			CreateDevice( NpadId.No3 );
			CreateDevice( NpadId.No4 );
			CreateDevice( NpadId.No5 );
			CreateDevice( NpadId.No6 );
			CreateDevice( NpadId.No7 );
			CreateDevice( NpadId.No8 );

			UpdateInternal( 0, 0 );

            ShowControllerSupportForSinglePlayer(false);

            InputManager.OnDeviceAttached += inputDevice =>
            {
                if (!Gunhouse.AppEntry.HasAppStarted())
                {
                    UnityEngine.Debug.Log("<color=red>we're not in it yet. don't pop the controller applet</color>");
                    return;
                } else
                {
                    UnityEngine.Debug.Log("<color=blue>it's ok to pop the applet.</color>");
                }
                UnityEngine.Debug.Log("Attached: " + inputDevice.Name);
                ShowControllerSupportForSinglePlayer(false);
            };
            /*InputManager.OnDeviceDetached += inputDevice =>
            {
                UnityEngine.Debug.Log("Detached: " + inputDevice.Name);
                ShowControllerSupportForSinglePlayer(false);
            };
            InputManager.OnActiveDeviceChanged += inputDevice =>
            {
                UnityEngine.Debug.Log("Switched: " + inputDevice.Name);
                ShowControllerSupportForSinglePlayer(false);
            };*/
        }


		public ControllerSupportResultInfo ShowControllerApplet( bool suspendUnityThreads )
		{
			// See nn::hid::ControllerSupportArg::SetDefault() in the SDK documentation for details.
			var controllerSupportArg = new ControllerSupportArg();

			// Values below are the defaults set by this.
			controllerSupportArg.SetDefault();

			// The minimum number of players that will get wireless controller connections.
			// Must be set to 0 if you want to allow someone to play in handheld mode only.
			// Ignored in single-player mode.
			// controllerSupportArg.playerCountMin = 0;

			// The maximum number of players that will get wireless controller connections.
			// The maximum allowed value appears to be 4. Anything over that will cause a crash.
			// Ignored in single-player mode.
			// controllerSupportArg.playerCountMax = 4;

			// Specifies whether to maintain the connection of controllers that are already connected.
			// Specify false to disconnect all controllers.
			// controllerSupportArg.enableTakeOverConnection = true;

			// Specifies whether to left-justify the controller numbers when controller support is ended.
			// When false is specified, there may be gaps in controller numbers when controller support is ended.
			// Ignored in single-player mode.
			// controllerSupportArg.enableLeftJustify = true;

			// Specifies whether to permit actions when both controllers are being held in a dual-controller grip.
			// When false is specified, actions cannot be made when both controllers are being held in a dual-controller grip.
			// This is designed for times like during local communication when you want to prohibit the dual-controller grip.
			// controllerSupportArg.enablePermitJoyDual = true;

			// Specifies whether to start controller support in single-player mode.
			// Enable this if your game is single-player.
			// controllerSupportArg.enableSingleMode = false;

			// Specifies whether to use colors to identify the individual controller numbers shown in the controller support UI.
			// controllerSupportArg.enableIdentificationColor = false;

			// Specifies the colors to use to identify the individual controller numbers shown in the controller support UI.
			// If enableIdentificationColor is false, the values specified here will not be applied.
			// controllerSupportArg.identificationColor = new nn.util.Color4u8[4];

			// Specifies whether to use explanatory text for the individual controller numbers shown in the controller support UI.
			// controllerSupportArg.enableExplainText = false;

			// The text to use for the individual controller numbers shown in the controller support UI.
			// You can specify up to 32 characters.
			// If enableExplainText is false, the values specified here will not be applied.
			// Check how the text actually displays to make sure it is not too long and otherwise displays appropriately.
			// controllerSupportArg.explainText = new byte[ExplainTextSize];
			// This field is a private data blob and must be set with:
			// ControllerSupport.SetExplainText( ref controllerSupportArg, "Player 1", NpadId.No1 );

			controllerSupportArg.enableIdentificationColor = true;
			controllerSupportArg.identificationColor[0].Set( 255, 100, 100, 255 );
			controllerSupportArg.identificationColor[1].Set( 100, 255, 100, 255 );
			controllerSupportArg.identificationColor[2].Set( 100, 100, 255, 255 );
			controllerSupportArg.identificationColor[3].Set( 255, 100, 255, 255 );

			controllerSupportArg.enableExplainText = true;
			ControllerSupport.SetExplainText( ref controllerSupportArg, "Player 1", NpadId.No1 );
			ControllerSupport.SetExplainText( ref controllerSupportArg, "Player 2", NpadId.No2 );
			ControllerSupport.SetExplainText( ref controllerSupportArg, "Player 3", NpadId.No3 );
			ControllerSupport.SetExplainText( ref controllerSupportArg, "Player 4", NpadId.No4 );

			return ShowControllerApplet( controllerSupportArg, suspendUnityThreads );
		}


		public ControllerSupportResultInfo ShowControllerApplet( ControllerSupportArg controllerSupportArg, bool suspendUnityThreads )
		{
			// ControllerSupportResultInfo has two fields:
			// byte playerCount;  The number of players determined by controller support.
			// NpadId selectedId; The NpadIdType selected in single-player mode.
			var controllerSupportResultInfo = new ControllerSupportResultInfo();
			controllerSupportResultInfo.playerCount = 0;
			controllerSupportResultInfo.selectedId = NpadId.Invalid;

#if !UNITY_EDITOR
			UnityEngine.Switch.Applet.Begin();
			ControllerSupport.Show( ref controllerSupportResultInfo, controllerSupportArg, suspendUnityThreads );
			UnityEngine.Switch.Applet.End();
#endif

			return controllerSupportResultInfo;
		}


		public NpadId ShowControllerSupportForSinglePlayer( bool suspendUnityThreads )
		{
			var controllerSupportArg = new ControllerSupportArg();
			controllerSupportArg.SetDefault();
			controllerSupportArg.enableSingleMode = true;
			var controllerSupportResultInfo = ShowControllerApplet( controllerSupportArg, suspendUnityThreads );
			return controllerSupportResultInfo.selectedId;
		}


		public int ShowControllerSupportForMultiPlayer( byte playerCountMin, byte playerCountMax, bool suspendUnityThreads )
		{
			var controllerSupportArg = new ControllerSupportArg();
			controllerSupportArg.SetDefault();
			controllerSupportArg.playerCountMin = playerCountMin;
			controllerSupportArg.playerCountMax = playerCountMax;
			var controllerSupportResultInfo = ShowControllerApplet( controllerSupportArg, suspendUnityThreads );
			return controllerSupportResultInfo.playerCount;
		}


		void CreateDebugDevice()
		{
			devices.Add( new NintendoSwitchDebugInputDevice() );
		}


		void CreateDevice( NpadId nPadId )
		{
			devices.Add( new NintendoSwitchInputDevice( nPadId ) );
		}


		void UpdateInternal( ulong updateTick, float deltaTime )
		{
			var deviceCount = devices.Count;
			for (var i = 0; i < deviceCount; i++)
			{
				var device = devices[i];

				var nintendoDevice = device as INintendoSwitchInputDevice;
				if (nintendoDevice != null)
				{
					nintendoDevice.UpdateInternalState();

					if (nintendoDevice.IsPresent != device.IsAttached)
					{
						if (nintendoDevice.IsPresent)
						{
							InputManager.AttachDevice( device );
						}
						else
						{
							InputManager.DetachDevice( device );
						}
					}
				}
			}
		}


		public override void Update( ulong updateTick, float deltaTime )
		{
			UpdateInternal( updateTick, deltaTime );
		}


		internal static bool Enable()
		{
#if UNITY_SWITCH && !UNITY_EDITOR
			InputManager.AddDeviceManager<NintendoSwitchInputDeviceManager>();
			return true;
#else
			return false;
#endif
		}
	}
	// @endcond
}

