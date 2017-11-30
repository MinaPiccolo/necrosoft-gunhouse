// NOTICE:
//
// This code contains proprietary information and is protected by national and 
// international copyright laws. It may not be disclosed to third parties or 
// copied or duplicated in any form, in whole or in part.
//
// The content herein is highly confidential and should be handled accordingly.
//
#if UNITY_SWITCH
namespace InControl
{
	using UnityEngine;
	using nn.hid;


	// @cond nodoc
	public class NintendoSwitchInputDevice : InputDevice, INintendoSwitchInputDevice
	{
		const float LowerDeadZone = 0.0f;
		const float UpperDeadZone = 1.0f;
		const float AnalogStickMax = AnalogStickState.Max;

		public bool IsPresent { get; private set; }
		public NpadId NPadId { get; private set; }
		public NpadStyle NPadStyle { get; private set; }
		public NpadJoyHoldType NPadHoldType { get; private set; }

		public bool EnableSixAxisSensors { get; set; }
		public readonly SixAxisSensorHandle[] SixAxisSensorHandles;
		public readonly SixAxisSensorState[] SixAxisSensorStates;
		public int SixAxisSensorCount { get; private set; }

		public readonly VibrationDeviceHandle[] VibrationDeviceHandles;
		public readonly VibrationDeviceInfo[] VibrationDeviceInfos;
		public int VibrationDeviceCount { get; private set; }

		NpadState state;


		public NintendoSwitchInputDevice( NpadId nPadId )
		{
			NPadId = nPadId;
			NPadStyle = Npad.GetStyleSet( NPadId );
			NPadHoldType = NpadJoy.GetHoldType();

			EnableSixAxisSensors = NintendoSwitchInputDeviceManager.EnableSixAxisSensors;
			SixAxisSensorHandles = new SixAxisSensorHandle[2];
			SixAxisSensorStates = new SixAxisSensorState[2];
			SixAxisSensorCount = 0;

			VibrationDeviceHandles = new VibrationDeviceHandle[2];
			VibrationDeviceInfos = new VibrationDeviceInfo[2];
			VibrationDeviceCount = 0;

			SortOrder = (int) NPadId;

			DeviceClass = InputDeviceClass.Controller;
			DeviceStyle = InputDeviceStyle.NintendoSwitch;

			IsPresent = false;

			Initialize();
		}


		void Initialize()
		{
			ClearInputState();
			ClearControls();
			SetupControls();
			SetupSixAxisSensors();
			SetupVibrationDevices();
		}


		void SetupControls()
		{
			switch (NPadStyle)
			{
			case NpadStyle.None:
			case NpadStyle.Invalid:
				Name = Meta = "No Controller";
				break;

			case NpadStyle.Handheld:
				Name = Meta = "Switch + Joy-Con Controllers";
				AddBasicControls();
				break;

			case NpadStyle.FullKey:
				Name = Meta = "Pro Controller";
				AddBasicControls();
				break;

			case NpadStyle.JoyDual:
				Name = Meta = "Dual Joy-Con Controllers";
				AddBasicControls();
				AddControl( InputControlType.LeftSL, "Left SL" );
				AddControl( InputControlType.LeftSR, "Left SR" );
				AddControl( InputControlType.RightSL, "Right SL" );
				AddControl( InputControlType.RightSR, "Right SR" );
				break;

			case NpadStyle.JoyLeft:
				Name = Meta = "Joy-Con (L) Controller";
				AddLeftStickControls();
				AddControl( InputControlType.LeftBumper, "L" );
				AddControl( InputControlType.LeftTrigger, "ZL" );
				AddControl( InputControlType.LeftSL, "SL" );
				AddControl( InputControlType.LeftSR, "SR" );
				AddControl( InputControlType.Minus, "Minus" );
				if (NPadHoldType == NpadJoyHoldType.Vertical)
				{
					AddDPadControls();
				}
				else
				{
					AddControl( InputControlType.Action1, "Down" );
					AddControl( InputControlType.Action2, "Right" );
					AddControl( InputControlType.Action3, "Left" );
					AddControl( InputControlType.Action4, "Up" );
				}
				break;

			case NpadStyle.JoyRight:
				Name = Meta = "Joy-Con (R) Controller";
				AddControl( InputControlType.RightBumper, "R" );
				AddControl( InputControlType.RightTrigger, "ZR" );
				AddControl( InputControlType.RightSL, "SL" );
				AddControl( InputControlType.RightSR, "SR" );
				AddControl( InputControlType.Plus, "Plus" );
				if (NPadHoldType == NpadJoyHoldType.Vertical)
				{
					AddRightStickControls();
					AddControl( InputControlType.Action1, "B" );
					AddControl( InputControlType.Action2, "A" );
					AddControl( InputControlType.Action3, "Y" );
					AddControl( InputControlType.Action4, "X" );
				}
				else
				{
					AddLeftStickControls();
					AddControl( InputControlType.Action1, "A" );
					AddControl( InputControlType.Action2, "X" );
					AddControl( InputControlType.Action3, "B" );
					AddControl( InputControlType.Action4, "Y" );
				}
				break;
			}
		}


		void AddBasicControls()
		{
			AddLeftStickControls();
			AddRightStickControls();
			AddDPadControls();
			AddControl( InputControlType.Action1, "B" );
			AddControl( InputControlType.Action2, "A" );
			AddControl( InputControlType.Action3, "Y" );
			AddControl( InputControlType.Action4, "X" );
			AddControl( InputControlType.LeftBumper, "L" );
			AddControl( InputControlType.RightBumper, "R" );
			AddControl( InputControlType.LeftTrigger, "ZL" );
			AddControl( InputControlType.RightTrigger, "ZR" );
			AddControl( InputControlType.Plus, "Plus" );
			AddControl( InputControlType.Minus, "Minus" );
		}


		void AddLeftStickControls()
		{
			AddControl( InputControlType.LeftStickLeft, "Left Stick Left", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickRight, "Left Stick Right", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickUp, "Left Stick Up", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickDown, "Left Stick Down", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickButton, "Left Stick Button" );
		}


		void AddRightStickControls()
		{
			AddControl( InputControlType.RightStickLeft, "Right Stick Left", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickRight, "Right Stick Right", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickUp, "Right Stick Up", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickDown, "Right Stick Down", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickButton, "Right Stick Button" );
		}


		void AddDPadControls()
		{
			AddControl( InputControlType.DPadUp, "DPad Up" );
			AddControl( InputControlType.DPadDown, "DPad Down" );
			AddControl( InputControlType.DPadLeft, "DPad Left" );
			AddControl( InputControlType.DPadRight, "DPad Right" );
		}


		void SetupSixAxisSensors()
		{
			for (var i = 0; i < SixAxisSensorCount; i++)
			{
				SixAxisSensor.Stop( SixAxisSensorHandles[i] );
			}

			if (EnableSixAxisSensors && HasNPadStyle)
			{
				SixAxisSensorCount = SixAxisSensor.GetHandles( SixAxisSensorHandles, 2, NPadId, NPadStyle );

				for (var i = 0; i < SixAxisSensorCount; i++)
				{
					SixAxisSensor.Start( SixAxisSensorHandles[i] );
				}
			}
			else
			{
				SixAxisSensorCount = 0;
			}
		}


		void UpdateSixAxisSensors()
		{
			for (var i = 0; i < SixAxisSensorCount; i++)
			{
				SixAxisSensor.GetState( ref SixAxisSensorStates[i], SixAxisSensorHandles[i] );
			}
		}


		private nn.util.Float4 sixAxisFloat4 = new nn.util.Float4();
		public void GetSixAxisSensorQuaternion( int index, ref Quaternion quaternion )
		{
			if (index >= 0 && index < SixAxisSensorCount)
			{
				
				SixAxisSensorStates[index].GetQuaternion( ref sixAxisFloat4 );
				quaternion.Set( sixAxisFloat4.x, sixAxisFloat4.z, sixAxisFloat4.y, -sixAxisFloat4.w );
			}
			else
			{
				quaternion = Quaternion.identity;
			}
		}


		public Quaternion GetSixAxisSensorQuaternion( int index )
		{
			if (index >= 0 && index < SixAxisSensorCount)
			{

				SixAxisSensorStates[index].GetQuaternion( ref sixAxisFloat4 );
				return new Quaternion( sixAxisFloat4.x, sixAxisFloat4.z, sixAxisFloat4.y, -sixAxisFloat4.w );
			}
			else
			{
				return Quaternion.identity;
			}
		}


		void SetupVibrationDevices()
		{
			if (HasNPadStyle)
			{
				VibrationDeviceCount = Vibration.GetDeviceHandles( VibrationDeviceHandles, 2, NPadId, NPadStyle );
				for (var i = 0; i < VibrationDeviceCount; i++)
				{
					Vibration.InitializeDevice( VibrationDeviceHandles[i] );
					Vibration.GetDeviceInfo( ref VibrationDeviceInfos[i], VibrationDeviceHandles[i] );
				}
			}
		}


		public void Vibrate( int index, VibrationValue vibrationValue )
		{
			if (index >= 0 && index < VibrationDeviceCount)
			{
				Vibration.SendValue( VibrationDeviceHandles[index], vibrationValue );
			}
		}


		void Vibrate( int index, float amplitude )
		{
			var vibrationValue = new VibrationValue
			{
				amplitudeLow = amplitude,
				frequencyLow = VibrationValue.FrequencyLowDefault,
				amplitudeHigh = amplitude,
				frequencyHigh = VibrationValue.FrequencyHighDefault
			};
			Vibrate( index, vibrationValue );
		}


		void Vibrate( int index, float amplitudeLow, float amplitudeHigh )
		{
			var vibrationValue = new VibrationValue
			{
				amplitudeLow = amplitudeLow,
				frequencyLow = VibrationValue.FrequencyLowDefault,
				amplitudeHigh = amplitudeHigh,
				frequencyHigh = VibrationValue.FrequencyHighDefault
			};
			Vibrate( index, vibrationValue );
		}


		void Vibrate( int index, int frequencyLow, float amplitudeLow, int frequencyHigh, float amplitudeHigh )
		{
			var vibrationValue = new VibrationValue
			{
				amplitudeLow = amplitudeLow,
				frequencyLow = frequencyLow,
				amplitudeHigh = amplitudeHigh,
				frequencyHigh = frequencyHigh
			};
			Vibrate( index, vibrationValue );
		}


		public override void Vibrate( float leftMotor, float rightMotor )
		{
			var angle = Mathf.Deg2Rad * Time.realtimeSinceStartup * 360.0f * 30.0f;
			var sinPulse = 0.5f + Mathf.Sin( angle ) * 0.5f;
			var cosPulse = 0.5f + Mathf.Cos( angle ) * 0.5f;
			Vibrate( 0, sinPulse * leftMotor, cosPulse * leftMotor );
			Vibrate( 1, sinPulse * rightMotor, cosPulse * rightMotor );
		}


		public void UpdateInternalState()
		{
			var oldNPadStyle = NPadStyle;
			var oldNPadHoldType = NPadHoldType;

			NPadStyle = Npad.GetStyleSet( NPadId );
			if (HasNPadStyle)
			{
				NPadHoldType = NpadJoy.GetHoldType();
				Npad.GetState( ref state, NPadId, NPadStyle );
				IsPresent = (state.attributes & NpadAttribute.IsConnected) != 0;
			}
			else
			{
				IsPresent = false;
			}

			if (oldNPadStyle != NPadStyle || oldNPadHoldType != NPadHoldType)
			{
				Initialize();
			}
		}


		bool HasNPadStyle
		{
			get
			{
				return NPadStyle != NpadStyle.None && NPadStyle != NpadStyle.Invalid;
			}
		}


		public override void Update( ulong updateTick, float deltaTime )
		{
			if (IsPresent)
			{
				UpdateControls( updateTick, deltaTime );
				UpdateSixAxisSensors();
			}
		}


		void UpdateControls( ulong updateTick, float deltaTime )
		{
			switch (NPadStyle)
			{
			case NpadStyle.None:
			case NpadStyle.Invalid:
				break;

			case NpadStyle.Handheld:
				UpdateBasicControls( updateTick, deltaTime );
				break;

			case NpadStyle.FullKey:
				UpdateBasicControls( updateTick, deltaTime );
				break;

			case NpadStyle.JoyDual:
				Name = Meta = "Dual Joy-Con Controllers";
				UpdateBasicControls( updateTick, deltaTime );
				UpdateWithState( InputControlType.LeftSL, GetButtonState( NpadButton.LeftSL ), updateTick, deltaTime );
				UpdateWithState( InputControlType.LeftSR, GetButtonState( NpadButton.LeftSR ), updateTick, deltaTime );
				UpdateWithState( InputControlType.RightSL, GetButtonState( NpadButton.RightSL ), updateTick, deltaTime );
				UpdateWithState( InputControlType.RightSR, GetButtonState( NpadButton.RightSR ), updateTick, deltaTime );
				break;

			case NpadStyle.JoyLeft:
				Name = Meta = "Joy-Con (L) Controller";
				UpdateWithState( InputControlType.LeftBumper, GetButtonState( NpadButton.L ), updateTick, deltaTime );
				UpdateWithState( InputControlType.LeftTrigger, GetButtonState( NpadButton.ZL ), updateTick, deltaTime );
				UpdateWithState( InputControlType.LeftSL, GetButtonState( NpadButton.LeftSL ), updateTick, deltaTime );
				UpdateWithState( InputControlType.LeftSR, GetButtonState( NpadButton.LeftSR ), updateTick, deltaTime );
				UpdateWithState( InputControlType.Minus, GetButtonState( NpadButton.Minus ), updateTick, deltaTime );
				if (NPadHoldType == NpadJoyHoldType.Vertical)
				{
					UpdateLeftStickControls( updateTick, deltaTime );
					UpdateDPadControls( updateTick, deltaTime );
				}
				else
				{
					UpdateClockwiseRotatedLeftStickControls( updateTick, deltaTime );
					UpdateWithState( InputControlType.Action1, GetButtonState( NpadButton.Left ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action2, GetButtonState( NpadButton.Down ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action3, GetButtonState( NpadButton.Up ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action4, GetButtonState( NpadButton.Right ), updateTick, deltaTime );
				}
				break;

			case NpadStyle.JoyRight:
				Name = Meta = "Joy-Con (R) Controller";
				UpdateWithState( InputControlType.RightBumper, GetButtonState( NpadButton.R ), updateTick, deltaTime );
				UpdateWithState( InputControlType.RightTrigger, GetButtonState( NpadButton.ZR ), updateTick, deltaTime );
				UpdateWithState( InputControlType.RightSL, GetButtonState( NpadButton.RightSL ), updateTick, deltaTime );
				UpdateWithState( InputControlType.RightSR, GetButtonState( NpadButton.RightSR ), updateTick, deltaTime );
				UpdateWithState( InputControlType.Plus, GetButtonState( NpadButton.Plus ), updateTick, deltaTime );
				if (NPadHoldType == NpadJoyHoldType.Vertical)
				{
					UpdateRightStickControls( updateTick, deltaTime );
					UpdateWithState( InputControlType.Action1, GetButtonState( NpadButton.B ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action2, GetButtonState( NpadButton.A ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action3, GetButtonState( NpadButton.Y ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action4, GetButtonState( NpadButton.X ), updateTick, deltaTime );
				}
				else
				{
					UpdateCounterclockwiseRotatedLeftStickControls( updateTick, deltaTime );
					UpdateWithState( InputControlType.Action1, GetButtonState( NpadButton.A ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action2, GetButtonState( NpadButton.X ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action3, GetButtonState( NpadButton.B ), updateTick, deltaTime );
					UpdateWithState( InputControlType.Action4, GetButtonState( NpadButton.Y ), updateTick, deltaTime );
				}
				break;
			}

			Commit( updateTick, deltaTime );
		}


		void UpdateBasicControls( ulong updateTick, float deltaTime )
		{
			UpdateLeftStickControls( updateTick, deltaTime );
			UpdateRightStickControls( updateTick, deltaTime );
			UpdateDPadControls( updateTick, deltaTime );

			UpdateWithState( InputControlType.Action1, GetButtonState( NpadButton.B ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action2, GetButtonState( NpadButton.A ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action3, GetButtonState( NpadButton.Y ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action4, GetButtonState( NpadButton.X ), updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftBumper, GetButtonState( NpadButton.L ), updateTick, deltaTime );
			UpdateWithState( InputControlType.RightBumper, GetButtonState( NpadButton.R ), updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftTrigger, GetButtonState( NpadButton.ZL ), updateTick, deltaTime );
			UpdateWithState( InputControlType.RightTrigger, GetButtonState( NpadButton.ZR ), updateTick, deltaTime );

			UpdateWithState( InputControlType.Plus, GetButtonState( NpadButton.Plus ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Minus, GetButtonState( NpadButton.Minus ), updateTick, deltaTime );
		}


		void UpdateLeftStickControls( ulong updateTick, float deltaTime )
		{
			var lsv = new Vector2( state.analogStickL.x / AnalogStickMax, state.analogStickL.y / AnalogStickMax );
			UpdateLeftStickWithValue( lsv, updateTick, deltaTime );
			UpdateWithState( InputControlType.LeftStickButton, GetButtonState( NpadButton.StickL ), updateTick, deltaTime );
		}


		void UpdateRightStickControls( ulong updateTick, float deltaTime )
		{
			var rsv = new Vector2( state.analogStickR.x / AnalogStickMax, state.analogStickR.y / AnalogStickMax );
			UpdateRightStickWithValue( rsv, updateTick, deltaTime );
			UpdateWithState( InputControlType.RightStickButton, GetButtonState( NpadButton.StickR ), updateTick, deltaTime );
		}

		void UpdateClockwiseRotatedLeftStickControls( ulong updateTick, float deltaTime )
		{
			var lsv = new Vector2( -state.analogStickL.y / AnalogStickMax, state.analogStickL.x / AnalogStickMax );
			UpdateLeftStickWithValue( lsv, updateTick, deltaTime );
			UpdateWithState( InputControlType.LeftStickButton, GetButtonState( NpadButton.StickL ), updateTick, deltaTime );
		}


		void UpdateCounterclockwiseRotatedLeftStickControls( ulong updateTick, float deltaTime )
		{
			var rsv = new Vector2( state.analogStickR.y / AnalogStickMax, -state.analogStickR.x / AnalogStickMax );
			UpdateLeftStickWithValue( rsv, updateTick, deltaTime );
			UpdateWithState( InputControlType.LeftStickButton, GetButtonState( NpadButton.StickR ), updateTick, deltaTime );
		}


		void UpdateDPadControls( ulong updateTick, float deltaTime )
		{
			UpdateWithState( InputControlType.DPadUp, GetButtonState( NpadButton.Up ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadDown, GetButtonState( NpadButton.Down ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadLeft, GetButtonState( NpadButton.Left ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadRight, GetButtonState( NpadButton.Right ), updateTick, deltaTime );
		}


		bool GetButtonState( NpadButton button )
		{
			return (state.buttons & button) != 0;
		}
	}
	// @endcond
}

#endif