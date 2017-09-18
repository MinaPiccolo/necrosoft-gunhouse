/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

using System.Text;
using System.Runtime.InteropServices;

namespace nn.account
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UserSelectionSettings
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Account.UserCountMax)]
        public Uid[] invalidUidList;

        [MarshalAs(UnmanagedType.U1)]
        public bool isSkipEnabled;
        [MarshalAs(UnmanagedType.U1)]
        public bool isNetworkServiceAccountRequired;
        [MarshalAs(UnmanagedType.U1)]
        public bool showSkipButton;
        [MarshalAs(UnmanagedType.U1)]
        public bool additionalSelect;

        public void SetDefault()
        {
            invalidUidList = new Uid[Account.UserCountMax];
            isSkipEnabled = false;
            isNetworkServiceAccountRequired = false;
            showSkipButton = false;
            additionalSelect = false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("skip:{0} netAccount:{1} skipButton:{2} addSelect:{3} ignore:[ ",
                isSkipEnabled, isNetworkServiceAccountRequired, showSkipButton, additionalSelect);
            for (int i = 0; i < Account.UserCountMax; i++)
            {
                if (invalidUidList[i] != Uid.Invalid)
                {
                    builder.Append(invalidUidList[i].ToString());
                    builder.Append(" ");
                }
                builder.Append("]");

            }
            return builder.ToString();
        }

        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(UserSelectionSettings lhs, UserSelectionSettings rhs) { return Nn.OperatorEquals(lhs, rhs); }
        public static bool operator !=(UserSelectionSettings lhs, UserSelectionSettings rhs) { return !(lhs == rhs); }
    }

    public static partial class Account
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result ShowUserSelector(ref Uid pOut, UserSelectionSettings arg)
        {
            return new Result();
        }

        public static Result ShowUserSelector(ref Uid pOut)
        {
            return new Result();
        }

        public static Result ShowUserCreator()
        {
            return new Result();
        }

#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserSelector0")]
        public static extern Result ShowUserSelector(ref Uid pOut, UserSelectionSettings arg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserSelector1")]
        public static extern Result ShowUserSelector(ref Uid pOut);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserCreator")]
        public static extern Result ShowUserCreator();
#endif

        public static Result ShowUserSelector(
            ref Uid pOut, UserSelectionSettings arg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserSelector(ref pOut, arg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserSelector(ref pOut, arg);
        }

        public static Result ShowUserSelector(ref Uid pOut, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserSelector(ref pOut);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserSelector(ref pOut);
        }

        public static Result ShowUserCreator(bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserCreator();
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserCreator();
        }

    }

}
