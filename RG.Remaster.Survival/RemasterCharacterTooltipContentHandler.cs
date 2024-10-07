using System.Collections.Generic;
using System.Text;
using I2.Loc;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using TMPro;
using UnityEngine;

namespace RG.Remaster.Survival;

public class RemasterCharacterTooltipContentHandler : TooltipContentHandler
{
	[Tooltip("Text handler of tooltip.")]
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private TextMeshProUGUI _additionalText;

	[SerializeField]
	private bool _specialHandlingOfDeadCharacterStatus = true;

	[SerializeField]
	private bool _useFullCharacterName = true;

	[SerializeField]
	private LocalizedString _statusInfoColor;

	[SerializeField]
	private EndOfDayListenerList _eodListenerList;

	private const string ADDITIONAL_TOOLTIP_FORMAT = "<color={1}>{0}</color>";

	[Tooltip("The 'dead' character status. This needs to be set so that if the character is dead that will be the only status displayed")]
	[SerializeField]
	private CharacterStatus _deadCharacterStatus;

	private Dictionary<Character, List<CharacterStatus>> _characterStatusesVisibleInUI;

	private void Awake()
	{
		if (_characterStatusesVisibleInUI == null)
		{
			_characterStatusesVisibleInUI = new Dictionary<Character, List<CharacterStatus>>();
		}
		_eodListenerList.RegisterOnEndOfDay(OnEndOfDay, "Reset", 999, this, forceOrder: true);
	}

	private void OnDestroy()
	{
		_eodListenerList.UnregisterOnEndOfDay(OnEndOfDay, "Reset");
	}

	public override void HandleContent(TooltipContent content)
	{
		if ((bool)(content as RemasterCharacterTooltipContent))
		{
			HandleCharacterTooltipContent((RemasterCharacterTooltipContent)content);
		}
	}

	private void HandleCharacterTooltipContent(RemasterCharacterTooltipContent content)
	{
		if (_characterStatusesVisibleInUI == null)
		{
			_characterStatusesVisibleInUI = new Dictionary<Character, List<CharacterStatus>>();
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (!_characterStatusesVisibleInUI.ContainsKey(content.Character))
		{
			_characterStatusesVisibleInUI.Add(content.Character, new List<CharacterStatus>());
			if (_text != null && content.Character != null)
			{
				if (_useFullCharacterName)
				{
					_text.text = content.Character.StaticData.FullName;
				}
				else
				{
					_text.text = content.Character.StaticData.Name;
				}
				if (content.Character.RuntimeData.CurrentStatuses != null)
				{
					if (_specialHandlingOfDeadCharacterStatus && _deadCharacterStatus != null && content.Character.RuntimeData.CurrentStatuses.Contains(_deadCharacterStatus))
					{
						stringBuilder.AppendLine(string.Empty);
						stringBuilder.AppendLine(_deadCharacterStatus.Name);
					}
					else
					{
						for (int i = 0; i < content.Character.RuntimeData.CurrentStatuses.Count; i++)
						{
							if (content.Character.RuntimeData.CurrentStatuses[i].IsVisibleInUI)
							{
								stringBuilder.AppendLine(string.Format("<color={1}>{0}</color>", content.Character.RuntimeData.CurrentStatuses[i].Name, _statusInfoColor));
								_characterStatusesVisibleInUI[content.Character].Add(content.Character.RuntimeData.CurrentStatuses[i]);
							}
						}
					}
				}
				_additionalText.text = stringBuilder.ToString();
				_additionalText.gameObject.GetComponent<Localize>().OnLocalize();
				_text.gameObject.GetComponent<Localize>().OnLocalize();
			}
		}
		else if (_text != null && _additionalText != null)
		{
			if (_useFullCharacterName)
			{
				_text.text = content.Character.StaticData.FullName;
			}
			else
			{
				_text.text = content.Character.StaticData.Name;
			}
			if (_specialHandlingOfDeadCharacterStatus && _deadCharacterStatus != null && _characterStatusesVisibleInUI[content.Character].Contains(_deadCharacterStatus))
			{
				stringBuilder.AppendLine(string.Empty);
				stringBuilder.AppendLine(_deadCharacterStatus.Name);
			}
			for (int j = 0; j < _characterStatusesVisibleInUI[content.Character].Count; j++)
			{
				stringBuilder.AppendLine(string.Format("<color={1}>{0}</color>", _characterStatusesVisibleInUI[content.Character][j].Name, _statusInfoColor));
			}
			_additionalText.text = stringBuilder.ToString();
			_additionalText.gameObject.GetComponent<Localize>().OnLocalize();
			_text.gameObject.GetComponent<Localize>().OnLocalize();
		}
		_additionalText.gameObject.SetActive(!string.IsNullOrEmpty(_additionalText.text));
	}

	private void OnEndOfDay()
	{
		foreach (Character key in _characterStatusesVisibleInUI.Keys)
		{
			_characterStatusesVisibleInUI[key].Clear();
			for (int i = 0; i < key.RuntimeData.CurrentStatuses.Count; i++)
			{
				if (key.RuntimeData.CurrentStatuses[i].IsVisibleInUI)
				{
					_characterStatusesVisibleInUI[key].Add(key.RuntimeData.CurrentStatuses[i]);
				}
			}
		}
	}
}
