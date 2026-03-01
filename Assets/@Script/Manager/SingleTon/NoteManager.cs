using System;
using System.Collections.Generic;
using Michsky.UI.Dark;
using NUnit.Framework.Internal;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
	public static NoteManager Instance;
	
	private const float ScrollWidth = 700;
	private const float ScrollHeight = 1000;
	
	private TextMeshProUGUI[] _texts;
	private Image[] _images;
	
	private RectTransform _scrollRect;
	private RectTransform _contentRect;
	private PressKeyEvent _escKey;
	private Canvas _myCanvas;
	
	private int _useImageCount = 0;
	private int _useTextCount = 0;
	private int _childCount = 0;
	
	[SerializeField] private NoteEntrySO _noteEntry;
	
	private void Awake()
	{
		#region Singleton

		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		
		#endregion
		
		_myCanvas = GetComponent<Canvas>();
		
		_scrollRect = gameObject.FindChild<RectTransform>("Scroll View", true);
		_contentRect = gameObject.FindChild<RectTransform>("Content", true);
		_escKey = gameObject.FindChild<PressKeyEvent>("ESC", true);
		
		_texts = gameObject.FindChilds<TextMeshProUGUI>("Content Text", true);
		_images = gameObject.FindChilds<Image>("Content Image", true);
		
		_scrollRect.rect.SetSize(ScrollWidth, ScrollHeight);
		_contentRect.anchoredPosition = new Vector2(-ScrollWidth / 2, 0);
		
		_escKey.gameObject.SetActive(false);
		_escKey.onPressEvent.AddListener(HideNoteUI);
		
		HideNoteUI();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			ShowNoteUI(_noteEntry);
		}
	}

	public void ShowNoteUI(NoteEntrySO noteEntry)
	{
		_myCanvas.enabled = true;
		_escKey.gameObject.SetActive(true);
		
		GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Dialogue, true);
		
		foreach (NoteContent content in noteEntry.Contents)
		{
			switch (content.Type)
			{
				case NoteContent.EntryType.Image:
					Image image = _images[_useImageCount++];
					image.sprite = (content as NoteImage)?.Image;
					SetAspect(image);
					DisplayContent(image);
					break;
				case NoteContent.EntryType.Text:
					TextMeshProUGUI text = _texts[_useTextCount++];
					text.text = (content as NoteText)?.Text;
					DisplayContent(text);
					break;
			}
		}
	}

	private void HideNoteUI()
	{
		_myCanvas.enabled = false;
		_escKey.gameObject.SetActive(false);
		
		GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Dialogue, false);
		
		SetGraphicsVisibility<TextMeshProUGUI>(_texts, false);
		SetGraphicsVisibility<Image>(_images, false);	
		
		_useImageCount = _useTextCount = _childCount = 0;
	}

	private void SetAspect(Image image)
	{
		LayoutElement layout = image.GetComponent<LayoutElement>();
		
		float aspectRatio = image.sprite.rect.height / image.sprite.rect.width;
		
		layout.preferredWidth = ScrollWidth;
		layout.preferredHeight = ScrollWidth * aspectRatio;
	}

	private void DisplayContent(Graphic content)
	{
		content.transform.SetSiblingIndex(_childCount++);
		content.gameObject.SetActive(true);
	}

	private void SetGraphicsVisibility<T>(T[] array, bool flag) where T : Graphic
	{
		foreach (T graphic in array)
		{
			graphic.gameObject.SetActive(flag);
		}
	}
}
