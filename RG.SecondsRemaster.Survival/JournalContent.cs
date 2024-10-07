using System;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public abstract class JournalContent
{
	public const int MIN_DISPLAY_PRIORITY = int.MinValue;

	public const int MAX_DISPLAY_PRIORITY = int.MaxValue;

	[SerializeField]
	protected EJournalContentType type;

	[SerializeField]
	[Range(-2.1474836E+09f, 2.1474836E+09f)]
	protected int displayPriority;

	[SerializeField]
	protected int displayOrder;

	[SerializeField]
	protected TextJournalGroupId groupId;

	private JournalContentDisplayer _displayer;

	public int DisplayPriority => displayPriority;

	public int DisplayOrder
	{
		get
		{
			return displayOrder;
		}
		set
		{
			displayOrder = value;
		}
	}

	public EJournalContentType Type => type;

	public JournalContentDisplayer Displayer
	{
		get
		{
			return _displayer;
		}
		set
		{
			_displayer = value;
		}
	}

	public TextJournalGroupId GroupId
	{
		get
		{
			return groupId;
		}
		set
		{
			groupId = value;
		}
	}

	protected JournalContent(SaveEvent saveEvent)
	{
		if (saveEvent == null)
		{
			Debug.LogError("This constructor can be used only for deserialization from SaveEvent");
		}
	}

	protected JournalContent(int displayPriority)
	{
		this.displayPriority = displayPriority;
		this.displayPriority = Mathf.Clamp(this.displayPriority, int.MinValue, int.MaxValue);
	}

	public virtual string Serialize()
	{
		return JsonUtility.ToJson(new JournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type
		});
	}

	public virtual void Deserialize(string data, SaveEvent saveEvent)
	{
		JournalContentWrapper wrapper = JsonUtility.FromJson<JournalContentWrapper>(data);
		DeserializeBaseWrapper(wrapper, saveEvent);
	}

	protected void DeserializeBaseWrapper(JournalContentWrapper wrapper, SaveEvent saveEvent)
	{
		displayOrder = wrapper.DisplayOrder;
		displayPriority = wrapper.DisplayPriority;
		groupId = ((!string.IsNullOrEmpty(wrapper.GroupId)) ? ((TextJournalGroupId)saveEvent.GetReferenceObjectByID(wrapper.GroupId)) : null);
		type = wrapper.Type;
	}
}
