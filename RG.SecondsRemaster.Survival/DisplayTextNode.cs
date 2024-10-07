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

[Node(true, "Legacy/Display Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal)
})]
public class DisplayTextNode : MessageNode
{
	public const string ID = "DisplayTextNode";

	private const int OFFSET = 4;

	[SerializeField]
	private LocalizedString _text;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private int _counter;

	public override string GetID => "DisplayTextNode";

	public override Node Create(Vector2 pos)
	{
		DisplayTextNode displayTextNode = ScriptableObject.CreateInstance<DisplayTextNode>();
		displayTextNode.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayTextNode.name = "Display Text";
		displayTextNode.CreateMutliInput("In", "Flow");
		displayTextNode.CreateInput("Term", "LocalizedString");
		displayTextNode.CreateInput("Character", "Character");
		displayTextNode.CreateInput("Item", "Item");
		displayTextNode.CreateOutput("Out", "Flow");
		return displayTextNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayTextNode obj = (DisplayTextNode)Create(rect.position + new Vector2(20f, 20f));
		obj._text = _text;
		obj._character = _character;
		obj._item = _item;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	private void AddNewVariable()
	{
		CreateInput("Var " + _counter, ObjectConnection.ID);
		_counter++;
	}

	private void RemoveVariable()
	{
		if (Inputs.Count > 4)
		{
			Inputs[Inputs.Count - 1].Delete();
			_counter--;
		}
	}

	protected override void OnNodeValidate()
	{
	}

	private List<int> GetLocalVariables(NodeCanvas canvas)
	{
		if (Inputs.Count > 4)
		{
			List<int> list = new List<int>(Inputs.Count - 4);
			for (int i = 4; i < Inputs.Count; i++)
			{
				if (Inputs[i].connection.typeID == "Bool")
				{
					bool currentValue = false;
					GetInputValue(Inputs[i], ref currentValue, canvas);
					list.Add(Convert.ToInt32(currentValue));
				}
				else if (Inputs[i].connection.typeID == "Int")
				{
					int currentValue2 = 0;
					GetInputValue(Inputs[i], ref currentValue2, canvas);
					list.Add(currentValue2);
				}
				else if (Inputs[i].connection.typeID == "PlayerDecision")
				{
					PlayerDecision currentValue3 = null;
					GetInputValue(Inputs[i], ref currentValue3, canvas);
					list.Add(currentValue3.ChoosenNumber);
				}
				else
				{
					list.Add(-1);
					Debug.LogError("Error in DisplayTextNode - wrong connection!!!");
				}
			}
			return list;
		}
		return null;
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _text, canvas);
		GetInputValue(Inputs[2], ref _character, canvas);
		GetInputValue(Inputs[3], ref _item, canvas);
		TextJournalContent textJournalContent = new TextJournalContent(_text, 0);
		textJournalContent.Characters = new List<Character> { _character };
		textJournalContent.Items = new List<IItem> { _item };
		textJournalContent.LocalVariablesInts = GetLocalVariables(canvas);
		if (_text.ToString().Contains("EXPEDITION"))
		{
			textJournalContent.ExpeditionCharacter = ExpeditionManager.Instance.GetExpeditionCharacter();
		}
		SecondsEventManager.AddJournalContent(base.ParentCanvas, textJournalContent);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
