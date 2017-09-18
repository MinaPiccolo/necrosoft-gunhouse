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
    public enum GestureType
    {
        Idle,
        Complete,
        Cancel,
        Touch,
        Press,
        Tap,
        Pan,
        Swipe,
        Pinch,
        Rotate,
    };

    public enum GestureDirection
    {
        None,
        Left,
        Up,
        Right,
        Down,
    };

    [Flags]
    public enum GestureAttribute
    {
        IsNewTouch = 0x1 << 4,
        IsDoubleTap = 0x1 << 8,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GesturePoint
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return String.Format("({0} {1})", this.x, this.y);
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct GestureState
    {
        public long eventNumber;
        public long contextNumber;
        public int _type;
        public int _direction;
        public int x;
        public int y;
        public int deltaX;
        public int deltaY;
        public nn.util.Float2 velocity;
        public GestureAttribute attributes;
        public float scale;
        public float rotationAngle;
        public int pointCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public GesturePoint[] points;

        public void SetDefault()
        {
            points = new GesturePoint[4];
        }

        public GestureType type { get { return (GestureType)this._type; } }

        public GestureDirection direction { get { return (GestureDirection)_direction; } }

        public bool isDoubleTap
        {
            get
            {
                return ((attributes & GestureAttribute.IsDoubleTap) == GestureAttribute.IsDoubleTap);
            }
        }

        public override string ToString()
        {
            return String.Format(
                "event:{0} con:{1} type:{2} dir:{3} pos:({4} {5}) delta:({6} {7}) vel:{8} attr:{9} scale:{10} rotA:{11} count:{12} p0:{13} p1:{14} p2:{15} p3:{16}",
                this.eventNumber, this.contextNumber, this.type, this.direction,
                this.x, this.y, this.deltaX, this.deltaY, this.velocity,
                this.attributes, this.scale, this.rotationAngle,
                this.pointCount, this.points[0], this.points[1], this.points[2], this.points[3]);
        }
    }

    public static partial class Gesture
    {
        public const int PointCountMax = 4;
        public const int StateCountMax = 16;

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeGesture")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetGestureStates")]
        public static extern int GetStates([Out] GestureState[] pOutValues, int count);
    }
}
