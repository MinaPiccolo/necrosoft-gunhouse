/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

namespace nn.irsensor
{
    public static partial class IrCamera
    {
        public static readonly nn.ErrorRange ResultIrsensorUnavailable = new nn.ErrorRange(205, 110, 120);
        public static readonly nn.ErrorRange ResultIrsensorUnconnected = new nn.ErrorRange(205, 110, 111);
        public static readonly nn.ErrorRange ResultIrsensorUnsupported = new nn.ErrorRange(205, 111, 112);
        public static readonly nn.ErrorRange ResultIrsensorNotReady = new nn.ErrorRange(205, 120, 121);
        public static readonly nn.ErrorRange ResultIrsensorDeviceError = new nn.ErrorRange(205, 122, 140);
    }
}
