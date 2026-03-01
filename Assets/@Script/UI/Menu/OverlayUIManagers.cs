using UnityEngine;

public class OverlayUIManagers : MonoBehaviour
{
	public static OverlayUIManagers Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		gameObject.FindChild<UI_SavePanel>("SavePannelManager", false);
		gameObject.FindChild<UI_PauseMenuPopup>("PauseMenuManager", false);
		gameObject.FindChild<NoteManager>("NoteManager", false);
	}
	
	public static UI_SavePanel UISavePannel { get; private set; }
	public static UI_PauseMenuPopup UIPauseMenuPopup { get; private set; }
	public static NoteManager Note { get; private set; }
}
