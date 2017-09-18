/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

namespace nn.hid
{
    public static partial class NpadJoy
    {
        public static readonly ErrorRange ResultDualConnected = new ErrorRange(202, 601, 602);
        public static readonly ErrorRange ResultSameJoyTypeConnected = new ErrorRange(202, 602, 603);
    }
        public static partial class Napd
    {
        public static readonly ErrorRange ResultColorNotAvailable = new ErrorRange(202, 603, 604);
    }
}