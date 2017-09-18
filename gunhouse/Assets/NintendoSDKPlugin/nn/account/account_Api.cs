/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

using System.Runtime.InteropServices;

namespace nn.account
{
    public static partial class Account
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result GetUserCount(ref int pOutCount)
        {
            pOutCount = 0;
            return new Result();
        }

        public static Result GetUserExistence(ref bool pOutExistence, Uid user)
        {
            pOutExistence = false;
            return new Result();
        }

        public static Result ListAllUsers(ref int pOutActualLength, Uid[] outUsers)
        {
            pOutActualLength = 0;
            return new Result();
        }

        public static Result ListOpenUsers(ref int pOutActualLength, Uid[] outUsers)
        {
            pOutActualLength = 0;
            return new Result();
        }

        public static Result GetLastOpenedUser(ref Uid pOutUser)
        {
            return new Result();
        }

        public static Result GetNickname(ref Nickname pOut, Uid user)
        {
            return new Result();
        }

        public static Result LoadProfileImage(ref int pOutActualSize, byte[] outImage, Uid user)
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_GetUserCount")]
        public static extern Result GetUserCount(ref int pOutCount);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_GetUserExistence")]
        public static extern Result GetUserExistence(ref bool pOutExistence, Uid user);

        public static Result ListAllUsers(ref int pOutActualLength, Uid[] outUsers)
        {
            return ListAllUsersImpl(ref pOutActualLength, outUsers, outUsers.Length);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ListAllUsers")]
        private static extern Result ListAllUsersImpl(
            ref int pOutActualLength, [Out] Uid[] outUsers, int arrayLength);

        public static Result ListOpenUsers(ref int pOutActualLength, Uid[] outUsers)
        {
            return ListOpenUsersImpl(ref pOutActualLength, outUsers, outUsers.Length);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ListOpenUsers")]
        private static extern Result ListOpenUsersImpl(
            ref int pOutActualLength, [Out] Uid[] outUsers, int arrayLength);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_GetLastOpenedUser")]
        public static extern Result GetLastOpenedUser(ref Uid pOutUser);

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_account_GetNickname")]
        public static extern Result GetNickname(ref Nickname pOut, Uid user);

        public static Result LoadProfileImage(ref int pOutActualSize, byte[] outImage, Uid user)
        {
            return LoadProfileImageImpl(ref pOutActualSize, outImage, outImage.Length, user);
        }

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_account_LoadProfileImage")]
        private static extern Result LoadProfileImageImpl(
            ref int pOutActualSize, byte[] outImage, int bufferSize, Uid user);
#endif
    }
}
