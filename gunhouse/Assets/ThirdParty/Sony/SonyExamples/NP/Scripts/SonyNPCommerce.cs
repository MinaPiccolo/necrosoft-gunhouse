using UnityEngine;
#if UNITY_PSP2
using UnityEngine.PSVita;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SonyNpCommerce : IScreen
{
	MenuLayout m_Menu;
	SonyNpCommerceEntitlements m_Entitlements;
	SonyNpCommerceStore m_Store;
	SonyNpCommerceInGameStore m_InGameStore;

	public SonyNpCommerce()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_Menu;
	}

	public void Initialize()
	{
		m_Menu = new MenuLayout(this, 450, 34);
		m_Store = new SonyNpCommerceStore();
		m_InGameStore = new SonyNpCommerceInGameStore();
		m_Entitlements = new SonyNpCommerceEntitlements();

		Sony.NP.Commerce.OnDownloadListStarted += OnSomeEvent;
		Sony.NP.Commerce.OnDownloadListFinished += OnSomeEvent;
	}

	public void OnEnter()
	{
		Sony.NP.Commerce.OnError += OnCommerceError;
		
		OnScreenLog.Add("Warning: For Commerce to work the npToolkit sample must be built using your own Conntent ID, titleid.dat and NP Title secret.");
        OnScreenLog.Add("         If you try to use the default setting you will not have the necessary permission to access the default npToolkit store page.");
	}

	public void OnExit()
	{
		Sony.NP.Commerce.OnError -= OnCommerceError;
	}

	static public Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode=Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			Sony.NP.ResultCode result = new Sony.NP.ResultCode();
			Sony.NP.Commerce.GetLastError(out result);
			if(result.lastError != Sony.NP.ErrorCode.NP_OK)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
				return result.lastError;
			}
		}

		return errorCode;
	}

	public void Process(MenuStack stack)
	{
		bool commerceReady = Sony.NP.User.IsSignedInPSN && !Sony.NP.Commerce.IsBusy();
		
		m_Menu.Update();

#if !UNITY_PS3
		if (m_Menu.AddItem("Store", commerceReady))
		{
			stack.PushMenu(m_Store.GetMenu());
		}
#endif

		if (m_Menu.AddItem("In Game Store"))
		{
			stack.PushMenu(m_InGameStore.GetMenu());
		}

		if (m_Menu.AddItem("Downloads"))
		{
			Sony.NP.Commerce.DisplayDownloadList();
		}

		if (m_Menu.AddItem("Entitlements"))
		{
			stack.PushMenu(m_Entitlements.GetMenu());
		}

#if UNITY_PSP2
		if (m_Menu.AddItem("Find Installed Content"))
		{
			EnumerateDRMContent();
		}
#endif
		
		if (m_Menu.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}

#if UNITY_PSP2
	int EnumerateDRMContentFiles(string contentDir)
	{
		int fileCount = 0;
		PSVitaDRM.ContentOpen(contentDir);

		string filePath = "addcont0:" + contentDir;

		OnScreenLog.Add("Found content folder: " + filePath);
		string[] files = Directory.GetFiles(filePath);
		OnScreenLog.Add(" containing " + files.Length + " files");
		foreach (string file in files)
		{
			OnScreenLog.Add("  " + file);
			fileCount++;
			// As a test, if the content file is an asset bundle load it's assets.
			if (file.Contains(".unity3d"))
			{
				AssetBundle bundle = AssetBundle.LoadFromFile(file);

#if UNITY_4_3
				Object[] assets = bundle.LoadAll();
#else
				Object[] assets = bundle.LoadAllAssets();
#endif
				OnScreenLog.Add("  Loaded " + assets.Length + " assets from asset bundle.");

				bundle.Unload(false);
			}
		}

		PSVitaDRM.ContentClose(contentDir);
		return fileCount;
	}
	
	void EnumerateDRMContent()
	{
		int fileCount = 0;
		PSVitaDRM.DrmContentFinder finder = new PSVitaDRM.DrmContentFinder();
		finder.dirHandle = -1;
		if (PSVitaDRM.ContentFinderOpen(ref finder))
		{
			fileCount += EnumerateDRMContentFiles(finder.contentDir);
			while (PSVitaDRM.ContentFinderNext(ref finder))
			{
				fileCount += EnumerateDRMContentFiles(finder.contentDir);
			};
			PSVitaDRM.ContentFinderClose(ref finder);
		}

		OnScreenLog.Add("Found " + fileCount + " files in installed DRM content");
	}
#endif

	void OnCommerceError(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("### OnCommerceError: " + msg.type);
		ErrorHandler();
	}

	void OnSomeEvent(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Event: " + msg.type);
	}
}
