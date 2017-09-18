/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

namespace nn.account
{
    public static partial class Account
    {
        public static readonly ErrorRange ResultCancelled = new ErrorRange(124, 0, 1);
        public static readonly ErrorRange ResultCancelledByUser = new ErrorRange(124, 1, 2);
        public static readonly ErrorRange ResultUserNotExist = new ErrorRange(124, 100, 101);
    }

    public static partial class NetworkServiceAccount
    {
        public static readonly ErrorRange ResultNetworkServiceAccountUnavailable = new ErrorRange(124, 200, 270);
        public static readonly ErrorRange ResultTokenCacheUnavailable = new ErrorRange(124, 430, 500);
        public static readonly ErrorRange ResultNetworkCommunicationError = new ErrorRange(124, 3000, 8192);
        public static readonly ErrorRange ResultSslService = new ErrorRange(123, 0, 5000);
    }
}