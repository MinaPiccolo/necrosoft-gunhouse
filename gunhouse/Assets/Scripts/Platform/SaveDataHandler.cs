#if UNITY_SWITCH
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using nn.account;
using nn.fs;
using UnityEngine.UI;

// This code is a for a saving sample that saves/loads a single string.

// Important: This code requires that in the Unity Editor you set PlayerSettings > Publishing Settings > Startup user account to 'Required'.
// This code does not check whether or not there is enough free space on disk to create save date. Instead, it relies on the 'Startup user account'
// setting to verify there is enough free space on device. If you would like to instead manage how your game creates save data space on device,
// see the NintendoSDK plugin and the NintendoSDK documentation.

public class SaveDataHandler : MonoBehaviour
{
    private static SaveDataHandler instance;

    private Uid userId; // user ID for the user account on the Nintendo Switch
    private const string mountName = "saveData";
    private string saveDataPath = mountName + ":/";
    private FileHandle fileHandle = new nn.fs.FileHandle();

    // Save journaling memory is used for each time files are created, deleted, or written.
    // The journaling memory is freed after nn::fs::CommitSaveData is called.
    // For any single time you save data, check the file size against your journaling size.
    // Check against the total save data size only when you want to be sure all files don't exceed the limit.
    // The variable journalSaveDataSize is only a value that is checked against in this code. The actual journal size is set in the
    // Unity editor in PlayerSettings > Publishing Settings > User account save data    
    private const int journalSaveDataSize = 16384;   // 16 KB. This value should be 32KB less than the journal size
                                                    // entered in PlayerSettings > Publishing Settings
    private const int loadBufferSize = 1024;  // 1 KB

    public void initialize()
    {
        nn.account.Account.Initialize();        
        nn.account.UserHandle userHandle = new nn.account.UserHandle();        
        nn.account.Account.OpenPreselectedUser(ref userHandle);
        nn.account.Account.GetUserId(ref userId, userHandle);       

        // mount save data        
        nn.Result result = nn.fs.SaveData.Mount(mountName, userId);
        
        //print out error (debug only) and abort if the filesystem couldn't be mounted 
        if (result.IsSuccess() == false)
        {
           Debug.Log("Critical Error: File System could not be mounted.");
           result.abortUnlessSuccess();
        }
    }

    void OnDestroy()
    {
        nn.fs.FileSystem.Unmount(mountName);
    }

    public static void Save(string dataToSave, string filename)
    {
        GetInstance().save(dataToSave, filename);
    }

    public void save(string dataToSave, string filename)
    {
        string filePath = saveDataPath + filename;

        byte[] dataByteArray;
        using (MemoryStream stream = new MemoryStream(journalSaveDataSize)) // the stream size must be less than or equal to the save journal size
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(dataToSave);
            stream.Close();
            dataByteArray = stream.GetBuffer();
        }

#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

        // If you only ever save the entire file, it may be simpler just to delete the file and create a new one every time you save.
        // Most of the functions return an nn.Result which can be used for debugging purposes.
        nn.fs.File.Delete(filePath);
        nn.fs.File.Create(filePath, journalSaveDataSize); //this makes a file the size of your save journal. You may want to make a file smaller than this.
        nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        nn.fs.File.Write(fileHandle, 0, dataByteArray, dataByteArray.LongLength, nn.fs.WriteOption.Flush); // Writes and flushes the write at the same time
        nn.fs.File.Close(fileHandle);
        nn.fs.SaveData.Commit(mountName); //you must commit the changes.

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
    }

    public static bool Load(ref string outputData, string filename)
    {
        return GetInstance().load(ref outputData, filename);
    }

    public bool load(ref string outputData, string filename)
    {
        nn.fs.EntryType entryType = 0; //init to a dummy value (C# requirement)
        nn.fs.FileSystem.GetEntryType(ref entryType, saveDataPath);
        nn.Result result = nn.fs.File.Open(ref fileHandle, saveDataPath + filename, nn.fs.OpenFileMode.Read);
        if (result.IsSuccess() == false)
        {
            return false;   // Could not open file. This can be used to detect if this is the first time a user has launched your game. 
                            // (However, be sure you are not getting this error due to your file being locked by another process, etc.)
        }
        byte[] loadedData = new byte[loadBufferSize]; 
        nn.fs.File.Read(fileHandle, 0, loadedData, loadBufferSize);
        nn.fs.File.Close(fileHandle);

        using (MemoryStream stream = new MemoryStream(loadedData))
        {
            BinaryReader reader = new BinaryReader(stream);
            outputData = reader.ReadString();
        }
        return true;
    }

    public static SaveDataHandler GetInstance()
    {
        if (instance != null) return instance;

        Debug.Log("instance is null. generating SaveDataHandler");

        GameObject newGameObject = new GameObject();
        instance = newGameObject.AddComponent<SaveDataHandler>();
        instance.initialize();
        return instance;
    }
}
#endif