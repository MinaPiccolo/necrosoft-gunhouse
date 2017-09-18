/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

using System;
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class IrCamera
    {
        public const int IntensityMax = 255;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public short x;
        public short y;
        public short width;
        public short height;

        public Rect(short x, short y, short width, short height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return String.Format("(x:{0} y:{1} w:{2} h:{3})",
                this.x, this.y, this.width, this.height);
        }
    }
}
