
Unity PS Vita Save Game API.

This example project demonstrates how to perform saving and loading of game state using the Unity PS Vita Save Game API

Project Folder Structure.

	Plugins/PSVita - Contains the SavedGames native plugin.
	Editor/SonyVitaSavedGamesPublishData - Contains required data for publishing.
	StreamingAssets - Contains the icon images for use with save games.
	SonyAssemblies - Contains the SonyVitaSavedGames managed interface to the SavedGames plugin.
	SonyExample/SavedGames - Contains a Unity scene which runs the scripts.
	SonyExample/SavedGames/Scripts - Contains the example scripts.
	SonyExample/Utils - Contains various utility scripts for use by the example.

The SonyVitaSavedGames managed assembly defines the following namespaces...

Sony.Vita.SavedGame.Main		Contains methods for initialising and updating the plugin.
Sony.Vita.SavedGame.LoadSave	Contains methods for saving and loading and managing saved games.

Sony.Vita.SavedGame.Main

	Methods.

		public static void Initialise()
		Initialises the plugin, call once.

		public static void Update()
		Updates the plugin, call once each frame.

Sony.Vita.SavedGame.LoadSave

	Structures.

		public struct SavedGameSlotParams	Structure defining a save game slot.
		{
			public string title;			Title name.
			public string subTitle;			Subtitle name.
			public string detail;			Detail info.
			public string iconPath;			Thumbnail icon path.
		};

	Events.

		OnGameSaved				Triggered when a game save has completed.
		OnGameLoaded			Triggered when a game load has completed.
		OnGameDeleted			Triggered when a game has been deleted.
		OnCanceled				Triggered when a save/load operation was canceled or aborted.
        OnSaveError				Triggered if a save failed, i.e. no space or missing device.
        OnLoadError				Triggered if a load failed, i.e. corrupt save slot.
		OnLoadNoData			Triggered if there was no data in the slot being loaded.

	Properties.

		public static bool IsDialogOpen
			Is the save/load dialog open? This also true when an error dialog is display at the end of an auto save/load.

		public static bool IsBusy
			Is the save/load process busy?

	Methods.

		public static bool SetEmptySlotIconPath(string iconPath)
			Set the path and filename for the empty slot icon.

		public static bool SetSlotCount(int slotCount)
			Set the number of available slots when using the save list methods.

		public static bool SaveGame(byte[] data, SavedGameSlotParams slotParams)
			Save a game to slot 0, use this method if your game only requires 1 save slot.
				
		public static bool LoadGame()
			Load a game from slot 0, use this method if you game only requires 1 save slot.
			When loading is complete the OnGameLoaded event will fire at which point GetLoadedGame() can be called to retrieve the data.

		public static bool AutoSaveGame(byte[] data, int slotNumber, SavedGameSlotParams slotParams)
			Save a game to the slot 'slotNumber', provides a silent save as long as there is no trouble.
				
		public static bool AutoLoadGame(int slotNumber)
			Load a game from the slot 'slotNumber, brings up no dialogs unless there are 'issues'
			
		public static bool SaveGameList(byte[] data, SavedGameSlotParams slotParams)
			Save a game with a slot selection dialog.

		public static bool LoadGameList()
			Load a game with a slot selection dialog.
			When loading is complete the OnGameLoaded event will fire at which point GetLoadedGame() can be called to retrieve the data.

		public static bool DeleteGameList()
			Delete a saved game.

		public static byte[] GetLoadedGame()
			Retrieve the data that was just loaded, use this method if you game only requires 1 save slot.
