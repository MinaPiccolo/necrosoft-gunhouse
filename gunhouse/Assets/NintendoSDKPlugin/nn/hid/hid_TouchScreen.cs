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

    [Flags]
    public enum TouchAttribute
    {
        Start = 0x1 << 0,
        End = 0x1 << 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchState
    {
        public long deltaTimeNanoSeconds;
        public TouchAttribute attributes;
        public int fingerId;
        public int x;
        public int y;
        public int diameterX;
        public int diameterY;
        public int rotationAngle;
        private int _reserved;

        public override string ToString()
        {
            return String.Format("fId:{0} pos:({1} {2}) dia:({3} {4}) rotA:{5} attr:{6} delta:{7}",
                this.fingerId, this.x, this.y, this.diameterX, this.diameterY, this.rotationAngle,
                this.attributes, this.deltaTimeNanoSeconds);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState1
    {
        public const int TouchCount = 1;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState2
    {
        public const int TouchCount = 2;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState3
    {
        public const int TouchCount = 3;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState4
    {
        public const int TouchCount = 4;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState5
    {
        public const int TouchCount = 5;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState6
    {
        public const int TouchCount = 6;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState7
    {
        public const int TouchCount = 7;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState8
    {
        public const int TouchCount = 8;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState9
    {
        public const int TouchCount = 9;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState10
    {
        public const int TouchCount = 10;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState11
    {
        public const int TouchCount = 11;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState12
    {
        public const int TouchCount = 12;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState13
    {
        public const int TouchCount = 13;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState14
    {
        public const int TouchCount = 14;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState15
    {
        public const int TouchCount = 15;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState16
    {
        public const int TouchCount = 16;
        public long samplingNumber;
        public int count;
        private int _reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TouchCount)]
        public TouchState[] touches;

        public void SetDefault()
        {
            touches = new hid.TouchState[TouchCount];
        }
    }

    public static partial class TouchScreen
    {
        public const int TouchCountMax = 16;
        public const int StateCountMax = 16;

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeTouchScreen")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState1")]
        public static extern void GetState(ref TouchScreenState1 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState2")]
        public static extern void GetState(ref TouchScreenState2 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState3")]
        public static extern void GetState(ref TouchScreenState3 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState4")]
        public static extern void GetState(ref TouchScreenState4 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState5")]
        public static extern void GetState(ref TouchScreenState5 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState6")]
        public static extern void GetState(ref TouchScreenState6 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState7")]
        public static extern void GetState(ref TouchScreenState7 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState8")]
        public static extern void GetState(ref TouchScreenState8 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState9")]
        public static extern void GetState(ref TouchScreenState9 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState10")]
        public static extern void GetState(ref TouchScreenState10 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState11")]
        public static extern void GetState(ref TouchScreenState11 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState12")]
        public static extern void GetState(ref TouchScreenState12 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState13")]
        public static extern void GetState(ref TouchScreenState13 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState14")]
        public static extern void GetState(ref TouchScreenState14 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState15")]
        public static extern void GetState(ref TouchScreenState15 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState16")]
        public static extern void GetState(ref TouchScreenState16 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates1")]
        public static extern int GetStates([Out] TouchScreenState1[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates2")]
        public static extern int GetStates([Out] TouchScreenState2[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates3")]
        public static extern int GetStates([Out] TouchScreenState3[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates4")]
        public static extern int GetStates([Out] TouchScreenState4[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates5")]
        public static extern int GetStates([Out] TouchScreenState5[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates6")]
        public static extern int GetStates([Out] TouchScreenState6[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates7")]
        public static extern int GetStates([Out] TouchScreenState7[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates8")]
        public static extern int GetStates([Out] TouchScreenState8[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates9")]
        public static extern int GetStates([Out] TouchScreenState9[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates10")]
        public static extern int GetStates([Out] TouchScreenState10[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates11")]
        public static extern int GetStates([Out] TouchScreenState11[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates12")]
        public static extern int GetStates([Out] TouchScreenState12[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates13")]
        public static extern int GetStates([Out] TouchScreenState13[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates14")]
        public static extern int GetStates([Out] TouchScreenState14[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates15")]
        public static extern int GetStates([Out] TouchScreenState15[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates16")]
        public static extern int GetStates([Out] TouchScreenState16[] pOutValues, int count);
    }
}
