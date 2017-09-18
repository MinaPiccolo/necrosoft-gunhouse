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
	// @cond nodoc
	public interface INintendoSwitchInputDevice
	{
		/// <summary>
		/// Update any internal state especially as related to presence.
		/// <seealso cref="IsPresent"/>
		/// </summary>
		void UpdateInternalState();


		/// <summary>
		/// Whether the device is physically present (connected, turned on, recognized by the system driver, etc.)
		/// </summary>
		/// <value>Returns <c>true</c> if is present; otherwise, <c>false</c>.</value>
		bool IsPresent { get; }
	}
	// @endcond
}

