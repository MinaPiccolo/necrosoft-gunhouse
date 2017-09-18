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

namespace nn.util
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4u8
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public void Set(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3})", this.r, this.g, this.b, this.a);
        }
    }
}
