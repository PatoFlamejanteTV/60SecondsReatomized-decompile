using System;
using System.Collections.Generic;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(true, "Text Nodes/Display Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal)
})]
public class DisplayTextNodeV2 : MessageNode
{
	public const string ID = "DisplayTextNodeV2";

	[SerializeField]
	private LocalizedString _text;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private int _priority;

	private const string NODE_NAME = "Display Text";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_TERM_NAME = "Term";

	private const string INPUT_PRIORITY_NAME = "Priority";

	private const string INPUT_CHARACTER_NAME = "Character";

	private const string INPUT_ITEM_NAME = "Item";

	private const string INPUT_VARIABLES_NAME = "Variables";

	private const string INPUT_TERMS_NAME = "Terms";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_TERM_INDEX = 1;

	private const int INPUT_PRIORITY_INDEX = 2;

	private const int INPUT_CHARACTER_INDEX = 3;

	private const int INPUT_ITEM_INDEX = 4;

	private const int INPUT_VARIABLES_INDEX = 5;

	private const int INPUT_TERMS_LIST_INDEX = 6;

	private const int OUTPUT_INDEX = 0;

	public override string GetID => "DisplayTextNodeV2";

	public override Node Create(Vector2 pos)
	{
		DisplayTextNodeV2 displayTextNodeV = ScriptableObject.CreateInstance<DisplayTextNodeV2>();
		displayTextNodeV.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayTextNodeV.name = "Display Text";
		displayTextNodeV.CreateMutliInput("In", "Flow");
		displayTextNodeV.CreateInput("Term", "LocalizedString");
		displayTextNodeV.CreateInput("Priority", "Int");
		displayTextNodeV.CreateInput("Character", "Character");
		displayTextNodeV.CreateInput("Item", "Item");
		displayTextNodeV.CreateInput("Variables", ListConnection.ID);
		displayTextNodeV.CreateInput("Terms", ListConnection.ID);
		displayTextNodeV.CreateOutput("Out", "Flow");
		return displayTextNodeV;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayTextNodeV2 obj = (DisplayTextNodeV2)Create(rect.position + new Vector2(20f, 20f));
		obj._text = _text;
		obj._character = _character;
		obj._item = _item;
		obj._priority = _priority;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _text, canvas);
		GetInputValue(Inputs[2], ref _priority, canvas);
		GetInputValue(Inputs[3], ref _character, canvas);
		GetInputValue(Inputs[4], ref _item, canvas);
		List<int> inputValue = GetInputValue<List<int>>(Inputs[5], canvas);
		List<LocalizedString> inputValue2 = GetInputValue<List<LocalizedString>>(Inputs[6], canvas);
		TextJournalContent textJournalContent = new TextJournalContent(_text, _priority);
		if (_character != null)
		{
			textJournalContent.Characters = new List<Character> { _character };
		}
		if (_item != null)
		{
			textJournalContent.Items = new List<IItem> { _item };
		}
		if (inputValue != null && inputValue.Count > 0)
		{
			textJournalContent.LocalVariablesInts = inputValue;
		}
		if (inputValue2 != null && inputValue2.Count > 0)
		{
			textJournalContent.Terms = inputValue2;
		}
		if (_text.ToString().Contains("EXPEDITION"))
		{
			textJournalContent.ExpeditionCharacter = ExpeditionManager.Instance.GetExpeditionCharacter();
		}
		SecondsEventManager.AddJournalContent(base.ParentCanvas, textJournalContent);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
