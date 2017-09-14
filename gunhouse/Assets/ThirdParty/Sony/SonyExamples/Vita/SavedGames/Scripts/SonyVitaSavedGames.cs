using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SonyVitaSavedGames : MonoBehaviour, IScreen
{
	// Save/load behavior control flags. Currently only used to say whether the "no space" dialog should show the continuable message or not.
	Sony.Vita.SavedGame.SaveLoad.ControlFlags controlFlags = Sony.Vita.SavedGame.SaveLoad.ControlFlags.NOSPACE_DIALOG_CONTINUABLE;

	int m_LevelNum = 0;
	int m_Score = 0;
	const int kSlotCount = 5;
	const int kAutoSaveSlot = 0;
	const int kDefaultSaveSlot = 0;
	const string kEmptySlotIconFileName = "SaveIconEmpty.png";
	const string kSlotIconFileName = "SaveIcon.png";
	const string kSlotTitle = "Save Game Test App";
	const string kSlotSubTitle = "Far far away";
	const string kSlotDetail = "This is just a test";
	//const string kSlotTitle = "ゲームテストアプリを保存";
	//const string kSlotSubTitle = "遠く離れ";
	//const string kSlotDetail = "これはただのテストです";
	
	MenuStack m_MenuStack;
	MenuLayout m_MenuMain;
	MenuLayout m_MenuSimpleSave;
	MenuLayout m_MenuListSave;

	void Start()
	{
		m_MenuMain = new MenuLayout(this, 500, 34);
		m_MenuSimpleSave = new MenuLayout(this, 500, 34);
		m_MenuListSave = new MenuLayout(this, 500, 34);
		m_MenuStack = new MenuStack();
		m_MenuStack.SetMenu(m_MenuMain);

		Sony.Vita.SavedGame.Main.enableInternalLogging = true;

		// Add log message handlers.
		Sony.Vita.SavedGame.Main.OnLog += OnLog;
		Sony.Vita.SavedGame.Main.OnLogWarning += OnLogWarning;
		Sony.Vita.SavedGame.Main.OnLogError += OnLogError;

		// Add message handlers for completion of asynchronous save/load operations.
		Sony.Vita.SavedGame.SaveLoad.OnGameSaved += OnSavedGameSaved;
		Sony.Vita.SavedGame.SaveLoad.OnGameLoaded += OnSavedGameLoaded;
		Sony.Vita.SavedGame.SaveLoad.OnGameDeleted += OnSavedGameDeleted;
		Sony.Vita.SavedGame.SaveLoad.OnCanceled += OnSavedGameCanceled;
		Sony.Vita.SavedGame.SaveLoad.OnSaveError += OnSaveError;
		Sony.Vita.SavedGame.SaveLoad.OnLoadError += OnLoadError;
		Sony.Vita.SavedGame.SaveLoad.OnLoadNoData += OnLoadNoData;

		// Note: Saving and loading is performed asynchronously in a separate thread and you cannot perform a save/load operation
		// while another is still in progress.
		//
		// To check when it is safe to call save or load you can poll Sony.Vita.SavedGame.SaveLoad.IsBusy until it becomes false
		// and/or wait for one of the completion message handlers to be called, e.g. Sony.Vita.SavedGame.SaveLoad.OnGameSaved etc.
		//
		// If you attempt to perform a save/load operation while one is already in progress then the error code SG_ERR_BUSY is
		// returned immediately.

		Sony.Vita.SavedGame.Main.Initialise();

		// Set the icon to use for empty save slots, just one of these unlike the icon for used slots which is specified for each save.
		Sony.Vita.SavedGame.SaveLoad.SetEmptySlotIconPath(Path.Combine(Application.streamingAssetsPath, kEmptySlotIconFileName));

		// Set the save slot count, only do this once.
		Sony.Vita.SavedGame.SaveLoad.SetSlotCount(kSlotCount);
	}

	public void OnEnter() {}
	public void OnExit() {}

	public void Process(MenuStack stack)
	{
		if (stack.GetMenu() == m_MenuMain)
		{
			MainMenu();
		}
		else if (stack.GetMenu() == m_MenuSimpleSave)
		{
			MenuSimpleSave();
		}
		else if (stack.GetMenu() == m_MenuListSave)
		{
			MenuListSave();
		}
	}

	// Class containing test data and methods for reading/writing from/to a byte buffer.
	class GameData
	{
		public int m_Level;
		public int m_Score;
		public byte[] m_Data;
		int m_DataSize;

		public GameData()
		{
			m_DataSize = 1*1024*1024;
			CreateTestData();
		}

		void CreateTestData()
		{
			m_Data = new byte[m_DataSize];
			for (int i = 0; i < m_DataSize; i++)
			{
				m_Data[i] = (byte)(i % 13);
			}
		}

		public byte[] WriteToBuffer()
		{
			int outputSize = m_DataSize + 16;
			System.IO.MemoryStream output = new MemoryStream(outputSize);
			System.IO.BinaryWriter writer = new BinaryWriter(output);
			writer.Write(m_Level);
			writer.Write(m_Score);
			writer.Write(m_Data);
			writer.Close();
			return output.GetBuffer();
		}

		public void ReadFromBuffer(byte[] buffer)
		{
			System.IO.MemoryStream input = new MemoryStream(buffer);
			System.IO.BinaryReader reader = new BinaryReader(input);
			m_Level = reader.ReadInt32();
			m_Score = reader.ReadInt32();
			m_Data = reader.ReadBytes(m_DataSize);
			reader.Close();
		}
	}

	void MainMenu()
	{
		bool isBusy = Sony.Vita.SavedGame.SaveLoad.IsBusy;

		m_MenuMain.Update();

		if (m_MenuMain.AddItem("With Confirmation Dialogs"))
		{
			m_MenuStack.PushMenu(m_MenuSimpleSave);
		}

		if (m_MenuMain.AddItem("With List Dialogs"))
		{
			m_MenuStack.PushMenu(m_MenuListSave);
		}

		if (m_MenuMain.AddItem("AutoSave", !isBusy))
		{
			AutoSave(kAutoSaveSlot);
		}

		if (m_MenuMain.AddItem("AutoLoad", !isBusy))
		{
			AutoLoad(kAutoSaveSlot);
		}

		if (m_MenuMain.AddItem("Delete Auto Save", !isBusy))
		{
			DeleteSlot(kAutoSaveSlot, false);
		}

		if (m_MenuMain.AddItem("Get Storage Info", !isBusy))
		{
			GetStorageInfo();
		}

		if (m_MenuMain.AddItem("Get Slot Info And Load", !isBusy))
		{
			GetSlotInfoAndLoad();
		}
	}

	// Demonstrates getting the current saved game storage usage.
	void GetStorageInfo()
	{
		// Get the quota (max storage space), this value is specified in the "Param File" section of the player settings.
		int quota = Sony.Vita.SavedGame.SaveLoad.GetQuota();

		// Get the current amount of used storage, note that this value is only valid when running from an installed package
		// and using the memory card for storage, e.g. not connected to host0:, otherwise it is always 0.
		int used = Sony.Vita.SavedGame.SaveLoad.GetUsedSize();

		OnScreenLog.Add("Storage used = " + used + "KB / " + quota + "KB");
	}

	// Demonstrates getting the metadata for save slots, useful if you want to implement a system that supports multiple save
	// slots and use your own UI instead of using Sony.Vita.SavedGame.SaveLoad.SaveGameList which uses the OS dialogs to display
	// the list.
	void GetSlotInfoAndLoad()
	{
		Sony.Vita.SavedGame.SaveLoad.SavedGameSlotInfo[] slotInfo = new Sony.Vita.SavedGame.SaveLoad.SavedGameSlotInfo[kSlotCount];

		// Get metadata for all save slots.
		int foundSlots = 0;
		for (int i=0; i < kSlotCount; i++)
		{
			Sony.Vita.SavedGame.ErrorCode res = Sony.Vita.SavedGame.SaveLoad.GetSlotInfo(i, out slotInfo[i]);
			if (res == Sony.Vita.SavedGame.ErrorCode.SG_OK)
			{
				// Show slots which are not empty.
				if (slotInfo[i].status != Sony.Vita.SavedGame.SaveLoad.SlotStatus.EMPTY)
				{
					OnScreenLog.Add("Slot " + slotInfo[i].slotNumber + " status: " + slotInfo[i].status);
					OnScreenLog.Add("  title: " + slotInfo[i].title);
					OnScreenLog.Add("  subTitle: " + slotInfo[i].subTitle);
					OnScreenLog.Add("  detail: " + slotInfo[i].detail);
					OnScreenLog.Add("  iconPath: " + slotInfo[i].iconPath);
					OnScreenLog.Add("  modifiedTime: " + slotInfo[i].modifiedTime);
					foundSlots++;
				}
			}
			else
			{
				// Some error occurred.
				Sony.Vita.SavedGame.ResultCode result = new Sony.Vita.SavedGame.ResultCode();
				Sony.Vita.SavedGame.SaveLoad.GetLastError(out result);
				OnScreenLog.Add("Slot " + i + " error: " + result.lastError + "(" + result.lastErrorSCE.ToString("X8") + ")");
			}
		}

		if (foundSlots > 0)
		{
			// As a test, load the last slot that has data...
			for (int i=kSlotCount - 1; i >= 0; i--)
			{
				if (slotInfo[i].status == Sony.Vita.SavedGame.SaveLoad.SlotStatus.AVAILABLE)
				{
					OnScreenLog.Add("Loading from slot " + slotInfo[i].slotNumber);

					// Use AutoLoadGame which silently loads the data without asking for user confirmation and only displays OS dialogs
					// if an error occurs. If you want confirmation then you could use LoadGame instead which uses the OS dialogs
					// to ask the user if they are sure they want to load the game.
					Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.AutoLoadGame(slotInfo[i].slotNumber);
					if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
					{
						OnScreenLog.Add("Load failed: " + result);
					}
					break;
				}
			}
		}
		else
		{
			OnScreenLog.Add("No slots in use");
		}

	}

	// Demonstrates deleting a save slot.
	void DeleteSlot(int slotNumber, bool useDialogs)
	{
		Sony.Vita.SavedGame.ErrorCode res = Sony.Vita.SavedGame.SaveLoad.Delete(slotNumber, useDialogs);
		if (res == Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Delete Slot " + slotNumber);
		}
		else
		{
			// Some error occurred.
			Sony.Vita.SavedGame.ResultCode result = new Sony.Vita.SavedGame.ResultCode();
			Sony.Vita.SavedGame.SaveLoad.GetLastError(out result);
			OnScreenLog.Add("Delete Slot failed with error: " + result.lastError + "(" + result.lastErrorSCE.ToString("X8") + ")");
		}
	}

	// Demonstrates auto-save, auto save writes the data silently with no OS confirmation dialogs.
	// OS dialogs are displayed if an error occurs.
	void AutoSave(int slotNumber)
	{
		// Construct some game data to save.
		GameData data = new GameData();
		data.m_Level = 1;
		data.m_Score = 123456789;
		byte[] bytes = data.WriteToBuffer();

		// Initialize the slot parameters.
		Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams slotParams = new Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams();
		slotParams.title = kSlotTitle;
		slotParams.subTitle = kSlotSubTitle;
		slotParams.detail = kSlotDetail;
		slotParams.iconPath = Path.Combine(Application.streamingAssetsPath, kSlotIconFileName);

		OnScreenLog.Add("Saving Game...");
		OnScreenLog.Add(" level: " + data.m_Level);
		OnScreenLog.Add(" score: " + data.m_Score);
		OnScreenLog.Add(" icon: " + slotParams.iconPath);
		OnScreenLog.Add(" size: " + bytes.Length / 1024 + "KB");

		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.AutoSaveGame(bytes, slotNumber, slotParams, controlFlags);
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Save failed: " + result);
		}
	}

	// Demonstrates auto-load, auto load reads the data silently with no OS confirmation dialogs.
	// OS dialogs are displayed if an error occurs.
	void AutoLoad(int slotNumber)
	{
		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.AutoLoadGame(slotNumber);
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Load failed: " + result);
		}
	}

	void MenuSimpleSave()
	{
		if (Sony.Vita.SavedGame.SaveLoad.IsDialogOpen == false)
		{
			m_MenuSimpleSave.Update();

			if (m_MenuSimpleSave.AddItem("Save Game"))
			{
				SimpleSave(kDefaultSaveSlot);
			}

			if (m_MenuSimpleSave.AddItem("Load Game"))
			{
				SimpleLoad(kDefaultSaveSlot);
			}

			if (m_MenuSimpleSave.AddItem("Delete Game"))
			{
				DeleteSlot(kAutoSaveSlot, true);
			}

			if (m_MenuSimpleSave.AddItem("Back"))
			{
				m_MenuStack.PopMenu();
			}
		}
	}

	// Demonstrates a simple save that saves data with an OS user confirmation dialog.
	void SimpleSave(int slotNumber)
	{
		// Construct some game data to save.
		GameData data = new GameData();
		data.m_Level = 1;
		data.m_Score = 123456789;
		byte[] bytes = data.WriteToBuffer();

		Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams slotParams = new Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams();
		slotParams.title = kSlotTitle;
		slotParams.subTitle = kSlotSubTitle;
		slotParams.detail = kSlotDetail;
		slotParams.iconPath = Path.Combine(Application.streamingAssetsPath, kSlotIconFileName);

		OnScreenLog.Add("Saving Game...");
		OnScreenLog.Add(" level: " + data.m_Level);
		OnScreenLog.Add(" score: " + data.m_Score);
		OnScreenLog.Add(" icon: " + slotParams.iconPath);
		OnScreenLog.Add(" size: " + bytes.Length / 1024 + "KB");

		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.SaveGame(bytes, slotNumber, slotParams, controlFlags);
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Save failed: " + result);
		}
	}

	// Demonstrates a simple load that loads data with an OS user confirmation dialog.
	void SimpleLoad(int slotNumber)
	{
		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.LoadGame(slotNumber);
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Load failed: " + result);
		}
	}

	void MenuListSave()
	{
		if (Sony.Vita.SavedGame.SaveLoad.IsDialogOpen == false)
		{
			m_MenuListSave.Update();

			if (m_MenuListSave.AddItem("Save Game"))
			{
				SaveGameList();
			}

			if (m_MenuListSave.AddItem("Load Game"))
			{
				LoadGameList();
			}

			if (m_MenuListSave.AddItem("Delete Game"))
			{
				DeleteGameList();
			}

			if (m_MenuListSave.AddItem("Back"))
			{
				m_MenuStack.PopMenu();
			}
		}
	}

	// Demonstrates saving data using the OS dialogs to let the user select the slot that is used.
	void SaveGameList()
	{
		// Construct some game data to save.
		GameData data = new GameData();
		m_LevelNum++;
		m_Score += 1000;

		data.m_Level = m_LevelNum;
		data.m_Score = m_Score;
		byte[] bytes = data.WriteToBuffer();

		Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams slotParams = new Sony.Vita.SavedGame.SaveLoad.SavedGameSlotParams();
		slotParams.title = kSlotTitle;
		slotParams.subTitle = kSlotSubTitle;
		slotParams.detail = kSlotDetail;
		slotParams.iconPath = Path.Combine(Application.streamingAssetsPath, kSlotIconFileName);

		OnScreenLog.Add("Saving Game...");
		OnScreenLog.Add(" level: " + data.m_Level);
		OnScreenLog.Add(" score: " + data.m_Score);
		OnScreenLog.Add(" icon: " + slotParams.iconPath);
		OnScreenLog.Add(" size: " + bytes.Length / 1024 + "KB");

		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.SaveGameList(bytes, slotParams, controlFlags);
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Save failed: " + result);
		}
	}

	// Demonstrates loading data using the OS dialogs to let the user select the slot that is used.
	void LoadGameList()
	{
		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.LoadGameList();
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Load failed: " + result);
		}
	}

	// Demonstrates deleting data using the OS dialogs to let the user select the slot to delete.
	void DeleteGameList()
	{
		Sony.Vita.SavedGame.ErrorCode result = Sony.Vita.SavedGame.SaveLoad.DeleteGameList();
		if (result != Sony.Vita.SavedGame.ErrorCode.SG_OK)
		{
			OnScreenLog.Add("Delete failed: " + result);
		}
	}

	void OnGUI()
	{
		MenuLayout activeMenu = m_MenuStack.GetMenu();
		activeMenu.GetOwner().Process(m_MenuStack);

		ShowDialogState();
	}

	void ShowDialogState()
	{
		GUIStyle style = GUI.skin.GetStyle("Label");
		style.fontSize = 16;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = false;

		// Is a dialog open?
		string state = Sony.Vita.SavedGame.SaveLoad.IsDialogOpen ? "Dlg Open" : "Dlg Closed";
		GUI.Label(new Rect(Screen.width - 100, 0, Screen.width - 1, style.lineHeight * 2), state, style);
	
		// Is the save game process busy, i.e. saving or loading.
		state = Sony.Vita.SavedGame.SaveLoad.IsBusy ? "Busy" : "Not Busy";
		GUI.Label(new Rect(Screen.width - 100, style.lineHeight * 2, Screen.width - 1, style.lineHeight * 2), state, style);
	}

	void OnSavedGameSaved(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Game Saved!");
	}

	void OnSavedGameLoaded(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Game Loaded...");
		byte[] bytes = Sony.Vita.SavedGame.SaveLoad.GetLoadedGame();
		if (bytes != null)
		{
			// Read the game data from the buffer.
			GameData data = new GameData();
			data.ReadFromBuffer(bytes);
			OnScreenLog.Add(" level: " + data.m_Level);
			OnScreenLog.Add(" score: " + data.m_Score);
		}
		else
		{
			OnScreenLog.Add(" ERROR: No data");
		}
	}

	void OnSavedGameDeleted(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Game deleted!");
	}
	
	void OnSavedGameCanceled(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Canceled");
	}

	void OnSaveError(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		Sony.Vita.SavedGame.ResultCode res = new Sony.Vita.SavedGame.ResultCode();
		Sony.Vita.SavedGame.SaveLoad.GetLastError(out res);
		OnScreenLog.Add("Failed to save: " + res.className + ": " + res.lastError + ", sce error 0x" + res.lastErrorSCE.ToString("X8"));
	}

	void OnLoadError(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		Sony.Vita.SavedGame.ResultCode res = new Sony.Vita.SavedGame.ResultCode();
		Sony.Vita.SavedGame.SaveLoad.GetLastError(out res);
		OnScreenLog.Add("Failed to load: " + res.className + ": " + res.lastError + ", sce error 0x" + res.lastErrorSCE.ToString("X8"));
	}

	void OnLoadNoData(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Nothing to load");
	}
	
	void Update ()
	{
		Sony.Vita.SavedGame.Main.Update();
	}

	void OnLog(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.Text);
	}

	void OnLogWarning(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("WARNING: " + msg.Text);
	}

	void OnLogError(Sony.Vita.SavedGame.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("ERROR: " + msg.Text);
	}
}
