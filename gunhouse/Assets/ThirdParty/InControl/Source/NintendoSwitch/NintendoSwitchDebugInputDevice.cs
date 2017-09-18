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
	using UnityEngine;
	using nn.hid;


	// @cond nodoc
	public class NintendoSwitchDebugInputDevice : InputDevice, INintendoSwitchInputDevice
	{
		const float LowerDeadZone = 0.0f;
		const float UpperDeadZone = 1.0f;
		const float AnalogStickMax = AnalogStickState.Max;

		public bool IsPresent { get; protected set; }

		DebugPadState state;


		public NintendoSwitchDebugInputDevice()
		{
			Name = "Nintendo Switch Debug Pad";
			Meta = "Nintendo Switch Debug Pad on SDEV";

			DeviceClass = InputDeviceClass.Controller;
			DeviceStyle = InputDeviceStyle.NintendoSwitch;

			IsPresent = false;

			CreateControls();
		}


		public void UpdateInternalState()
		{
			DebugPad.GetState( ref state );
			IsPresent = (state.attributes & DebugPadAttribute.IsConnected) != 0;
		}


		public override void Update( ulong updateTick, float deltaTime )
		{
			if (IsPresent)
			{
				UpdateControls( updateTick, deltaTime );
			}
		}


		void CreateControls()
		{
			AddControl( InputControlType.LeftStickLeft, "Left Stick Left", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickRight, "Left Stick Right", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickUp, "Left Stick Up", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.LeftStickDown, "Left Stick Down", LowerDeadZone, UpperDeadZone );

			AddControl( InputControlType.RightStickLeft, "Right Stick Left", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickRight, "Right Stick Right", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickUp, "Right Stick Up", LowerDeadZone, UpperDeadZone );
			AddControl( InputControlType.RightStickDown, "Right Stick Down", LowerDeadZone, UpperDeadZone );

			AddControl( InputControlType.Action1, "B" );
			AddControl( InputControlType.Action2, "A" );
			AddControl( InputControlType.Action3, "Y" );
			AddControl( InputControlType.Action4, "X" );

			AddControl( InputControlType.LeftBumper, "L" );
			AddControl( InputControlType.RightBumper, "R" );

			AddControl( InputControlType.LeftTrigger, "ZL" );
			AddControl( InputControlType.RightTrigger, "ZR" );

			AddControl( InputControlType.Start, "Start" );
			AddControl( InputControlType.Select, "Select" );

			AddControl( InputControlType.Plus, "Plus" );
			AddControl( InputControlType.Minus, "Minus" );

			AddControl( InputControlType.DPadUp, "DPad Up" );
			AddControl( InputControlType.DPadDown, "DPad Down" );
			AddControl( InputControlType.DPadLeft, "DPad Left" );
			AddControl( InputControlType.DPadRight, "DPad Right" );
		}


		void UpdateControls( ulong updateTick, float deltaTime )
		{
			var lsv = new Vector2( state.analogStickL.x / AnalogStickMax, state.analogStickL.y / AnalogStickMax );
			UpdateLeftStickWithValue( lsv, updateTick, deltaTime );

			var rsv = new Vector2( state.analogStickR.x / AnalogStickMax, state.analogStickR.y / AnalogStickMax );
			UpdateRightStickWithValue( rsv, updateTick, deltaTime );

			UpdateWithState( InputControlType.Action1, GetButtonState( DebugPadButton.B ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action2, GetButtonState( DebugPadButton.A ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action3, GetButtonState( DebugPadButton.Y ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Action4, GetButtonState( DebugPadButton.X ), updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftBumper, GetButtonState( DebugPadButton.L ), updateTick, deltaTime );
			UpdateWithState( InputControlType.RightBumper, GetButtonState( DebugPadButton.R ), updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftTrigger, GetButtonState( DebugPadButton.ZL ), updateTick, deltaTime );
			UpdateWithState( InputControlType.RightTrigger, GetButtonState( DebugPadButton.ZR ), updateTick, deltaTime );

			UpdateWithState( InputControlType.Start, GetButtonState( DebugPadButton.Start ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Select, GetButtonState( DebugPadButton.Select ), updateTick, deltaTime );

			UpdateWithState( InputControlType.Plus, GetButtonState( DebugPadButton.Start ), updateTick, deltaTime );
			UpdateWithState( InputControlType.Minus, GetButtonState( DebugPadButton.Select ), updateTick, deltaTime );

			UpdateWithState( InputControlType.DPadUp, GetButtonState( DebugPadButton.Up ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadDown, GetButtonState( DebugPadButton.Down ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadLeft, GetButtonState( DebugPadButton.Left ), updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadRight, GetButtonState( DebugPadButton.Right ), updateTick, deltaTime );

			Commit( updateTick, deltaTime );
		}


		bool GetButtonState( DebugPadButton button )
		{
			return (state.buttons & button) != 0;
		}
	}
	// @endcond
}

