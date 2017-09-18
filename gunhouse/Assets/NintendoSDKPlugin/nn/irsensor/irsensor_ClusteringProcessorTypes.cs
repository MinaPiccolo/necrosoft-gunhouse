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
using System.Text;
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class ClusteringProcessor
    {
        public const int StateCountMax = 5;
        public const int ObjectCountMax = 16;
        public const int ObjectPixelCountMax = 76800;
        public const int OutObjectPixelCountMax = 65535;
        public const long ExposureTimeMinNanoSeconds = 7 * 1000;
        public const long ExposureTimeMaxNanoSeconds = 600 * 1000;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClusteringProcessorConfig
    {
        public IrCameraConfig irCameraConfig;
        public Rect windowOfInterest;
        public int objectPixelCountMin;
        public int objectPixelCountMax;
        public int objectIntensityMin;
        [MarshalAs(UnmanagedType.U1)]
        public bool isExternalLightFilterEnabled;

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3} {4} {5})",
                this.irCameraConfig.ToString(), this.windowOfInterest.ToString(),
                this.objectPixelCountMin, this.objectPixelCountMax,
                this.objectIntensityMin, this.isExternalLightFilterEnabled);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClusteringData
    {
        public float averageIntensity;
        public util.Float2 centroid;
        public int pixelCount;
        public Rect bound;

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3})",
                this.averageIntensity, this.centroid.ToString(),
                this.pixelCount, this.bound.ToString());
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClusteringProcessorState
    {
        public long samplingNumber;
        public long timeStampNanoSeconds;
        public sbyte objectCount;
        public byte _reserved0;
        public byte _reserved1;
        public byte _reserved2;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ClusteringProcessor.ObjectCountMax)]
        public ClusteringData[] objects;

        public void SetDefault()
        {
            objects = new ClusteringData[ClusteringProcessor.ObjectCountMax];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("({0} {1} {2} {3})\n",
               this.samplingNumber, this.timeStampNanoSeconds,
               this.objectCount, this.ambientNoiseLevel.ToString());

            for (int i = 0; i < this.objectCount; i++)
            {
                builder.AppendFormat("object[{0}]:{1}\n", i, this.objects[i].ToString());
            }

            return builder.ToString();
        }
    }
}