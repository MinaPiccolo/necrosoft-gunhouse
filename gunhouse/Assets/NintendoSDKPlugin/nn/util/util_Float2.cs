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
    public struct Float2
    {
        public float x;
        public float y;

        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return String.Format("({0} {1})", this.x, this.y);
        }
    }
}
