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

namespace nn.hid
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationFileInfo
    {
        public uint metaDataSize;
        public ushort formatId;
        public ushort samplingRate;
        public uint dataSize;
        public int sampleLength;
        public int isLoop;
        public uint loopStartPosition;
        public uint loopEndPosition;
        public uint loopInterval;

        public override string ToString()
        {
            return String.Format("({0} {1}) SamplingRate:{2} DataSize:{3} SampleLength:{4} Loop:{5}({6} - {7}, {8})",
                this.metaDataSize, this.formatId, this.samplingRate, this.dataSize, this.sampleLength,
                this.isLoop, this.loopStartPosition, this.loopEndPosition, this.loopInterval);
        }
    };

    public struct VibrationFileParserContext
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public IntPtr[] _storage;
    };

    public static partial class VibrationFile
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ParseVibrationFile")]
        public static extern Result Parse(
            ref VibrationFileInfo pOutInfo, ref VibrationFileParserContext pOutContext, byte[] address, int fileSize);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_RetrieveVibrationValue")]
        public static extern void RetrieveValue(
            ref VibrationValue pOutValue, int position, ref VibrationFileParserContext pContext);
    }
}
