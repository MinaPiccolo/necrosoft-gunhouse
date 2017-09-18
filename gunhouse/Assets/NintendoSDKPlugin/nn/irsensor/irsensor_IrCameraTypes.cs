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
        public const int ImageWidth = 320;
        public const int ImageHeight = 240;
        public const int GainMin = 1;
        public const int GainMax = 16;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IrCameraHandle
    {
        public uint _handle;
    }

    public enum IrCameraStatus
    {
        Available,
        Unsupported,
        Unconnected,
    };

    public enum IrCameraAmbientNoiseLevel
    {
        Low,
        Middle,
        High,
        Unknown,
    };

    public enum IrCameraLightTarget
    {
        AllObjects,
        FarObjects,
        NearObjects,
        None,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct IrCameraConfig
    {
        public long exposureTimeNanoSeconds;
        public IrCameraLightTarget lightTarget;
        public int gain;
        [MarshalAs(UnmanagedType.U1)]
        public bool isNegativeImageUsed;

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3})",
                this.exposureTimeNanoSeconds, this.lightTarget, this.gain, this.isNegativeImageUsed);
        }
    }
}
