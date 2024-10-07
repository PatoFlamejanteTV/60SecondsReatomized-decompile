using System;
using System.Collections.Generic;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Text Nodes/Display Top Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent)
})]
public class DisplayTopTextNode : MessageNode
{
	public const string ID = "DisplayTopTextNode";

	private const int OFFSET = 4;

	[SerializeField]
	private LocalizedString _text;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private int _counter;

	public override string GetID => "DisplayTopTextNode";

	public override Node Create(Vector2 pos)
	{
		DisplayTopTextNode displayTopTextNode = ScriptableObject.CreateInstance<DisplayTopTextNode>();
		displayTopTextNode.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayTopTextNode.name = "Display Top Text";
		displayTopTextNode.CreateMutliInput("In", "Flow");
		displayTopTextNode.CreateInput("Term", "LocalizedString");
		displayTopTextNode.CreateInput("Character", "Character");
		displayTopTextNode.CreateInput("Item", "Item");
		displayTopTextNode.CreateOutput("Out", "Flow");
		return displayTopTextNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayTopTextNode obj = (DisplayTopTextNode)Create(rect.position + new Vector2(20f, 20f));
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
		TextJournalContent textJournalContent = new TextJournalContent(_text, int.MaxValue);
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
