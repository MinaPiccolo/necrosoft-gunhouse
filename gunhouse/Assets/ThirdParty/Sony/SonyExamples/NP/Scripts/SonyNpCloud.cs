using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SonyNpCloud : IScreen
{
	MenuLayout m_MenuCloud;
	SonyNpCloudTUS m_Tus;
	SonyNpCloudTSS m_Tss;

	public SonyNpCloud()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_MenuCloud;
	}

	public void OnEnter()
	{
	}

	public void OnExit()
	{
	}

	public void Process(MenuStack stack)
	{
		MenuCloud(stack);
	}

	public void Initialize()
	{
		m_MenuCloud = new MenuLayout(this, 550, 34);
		m_Tus = new SonyNpCloudTUS();
		m_Tss = new SonyNpCloudTSS();
	}

	public void MenuCloud(MenuStack stack)
	{
		m_MenuCloud.Update();

		if (m_MenuCloud.AddItem("Title User Storage"))
		{
			stack.PushMenu(m_Tus.GetMenu());
		}

		if (m_MenuCloud.AddItem("Title Small Storage"))
		{
			stack.PushMenu(m_Tss.GetMenu());
		}

		if (m_MenuCloud.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}
}
