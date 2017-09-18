/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

namespace nn.fs
{
    public static partial class FileSystem
    {
        public static readonly ErrorRange ResultHandledByAllProcess = new ErrorRange(2, 0, 1000);
        public static readonly ErrorRange ResultPathNotFound = new ErrorRange(2, 1, 2);
        public static readonly ErrorRange ResultPathAlreadyExists = new ErrorRange(2, 2, 3);
        public static readonly ErrorRange ResultTargetLocked = new ErrorRange(2, 7, 8);
        public static readonly ErrorRange ResultDirectoryNotEmpty = new ErrorRange(2, 8, 9);
        public static readonly ErrorRange ResultDirectoryStatusChanged = new ErrorRange(2, 13, 14);
        public static readonly ErrorRange ResultUsableSpaceNotEnough = new ErrorRange(2, 30, 46);
        public static readonly ErrorRange ResultUnsupportedSdkVersion = new ErrorRange(2, 50, 51);
        public static readonly ErrorRange ResultMountNameAlreadyExists = new ErrorRange(2, 60, 61);
        public static readonly ErrorRange ResultTargetNotFound = new ErrorRange(2, 1002, 1003);
    }

    public static partial class SaveData
    {
        public static readonly ErrorRange ResultUsableSpaceNotEnoughForSaveData = new ErrorRange(2, 31, 32);
    }

    public static partial class Host
    {
        public static readonly ErrorRange ResultSaveDataHostFileSystemCorrupted = new ErrorRange(2, 4441, 4460);
        public static readonly ErrorRange ResultSaveDataHostEntryCorrupted = new ErrorRange(2, 4442, 4443);
        public static readonly ErrorRange ResultSaveDataHostFileDataCorrupted = new ErrorRange(2, 4443, 4444);
        public static readonly ErrorRange ResultSaveDataHostFileCorrupted = new ErrorRange(2, 4444, 4445);
        public static readonly ErrorRange ResultInvalidSaveDataHostHandle = new ErrorRange(2, 4445, 4446);
        public static readonly ErrorRange ResultHostFileSystemCorrupted = new ErrorRange(2, 4701, 4720);
        public static readonly ErrorRange ResultHostEntryCorrupted = new ErrorRange(2, 4702, 4703);
        public static readonly ErrorRange ResultHostFileDataCorrupted = new ErrorRange(2, 4703, 4704);
        public static readonly ErrorRange ResultHostFileCorrupted = new ErrorRange(2, 4704, 4705);
        public static readonly ErrorRange ResultInvalidHostHandle = new ErrorRange(2, 4705, 4706);
    }

    public static partial class Rom
    {
        public static readonly ErrorRange ResultRomHostFileSystemCorrupted = new ErrorRange(2, 4241, 4260);
        public static readonly ErrorRange ResultRomHostEntryCorrupted = new ErrorRange(2, 4242, 4243);
        public static readonly ErrorRange ResultRomHostFileDataCorrupted = new ErrorRange(2, 4243, 4244);
        public static readonly ErrorRange ResultRomHostFileCorrupted = new ErrorRange(2, 4244, 4245);
        public static readonly ErrorRange ResultInvalidRomHostHandle = new ErrorRange(2, 4245, 4246);
    }
}
