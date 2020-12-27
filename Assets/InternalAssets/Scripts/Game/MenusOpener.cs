using UnityEngine;

public class MenusOpener : MonoBehaviour
{
	[SerializeField] private GameObject SettingsPanel;
	[SerializeField] private GameObject SavesPanel;

	public delegate void EventHandlerG(GameObject G);
	public static EventHandlerG OpenSavesMenuEvent;
	public static EventHandlerG OpenSettingsMenuEvent;
	public delegate void EventHandler();
	public static EventHandler ClosesMenusEvent;

	public static event EventHandler OnSavesMenuOpen;
	public static event EventHandler OnSavesMenuClose;

	private GameObject menuToOpen;

	private void Awake()
	{
		OpenSavesMenuEvent = OpenSavesMenu;
		OpenSettingsMenuEvent = OpenSettingsMenu;
		ClosesMenusEvent = CloseMenus;
	}

	public void OpenSavesMenu(GameObject G)
	{
		OnSavesMenuOpen?.Invoke();
		menuToOpen = G;
		SavesPanel.SetActive(true);
	}
	public void OpenSettingsMenu(GameObject G)
	{
		menuToOpen = G;
		SettingsPanel.SetActive(true);
	}

	public void CloseMenus()
	{
		SettingsPanel.SetActive(false);
		OnSavesMenuClose?.Invoke();
		SavesPanel.SetActive(false);
		if (menuToOpen)
		{
			menuToOpen.SetActive(true);
		}
	}
}
