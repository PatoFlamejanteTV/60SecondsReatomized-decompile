using System;
using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Consumables/Get Time Since Rationing Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(Goal),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas),
	typeof(ConditionEvent)
})]
public class GetTimeSinceRationingNode : ResourceNode
{
	public const string ID = "EE_GetTimeSinceRationingNode";

	private const string NODE_NAME = "Get Time Since Rationing";

	private const string INPUT_CHARACTER_NAME = "Character";

	private const string INPUT_CONSUMABLE_NAME = "Consumable Object";

	private const string INPUT_DAY_SHIFT_NAME = "Day Shift";

	private const string OUTPUT_LAST_RATIONING_NAME = "Last Rationing";

	private const int INPUT_CHARACTER_INDEX = 0;

	private const int INPUT_CONSUMABLE_INDEX = 1;

	private const int INPUT_INCLUDE_RATIONING_INDEX = 2;

	private const int OUTPUT_LAST_RATIONING_INDEX = 0;

	[SerializeField]
	private SecondsCharacter _character;

	[SerializeField]
	private SecondsConsumableRemedium _consumable;

	[SerializeField]
	private bool _dayShift = true;

	public override string GetID => "EE_GetTimeSinceRationingNode";

	public override Node Create(Vector2 pos)
	{
		GetTimeSinceRationingNode getTimeSinceRationingNode = ScriptableObject.CreateInstance<GetTimeSinceRationingNode>();
		getTimeSinceRationingNode.rect = new Rect(pos.x, pos.y, 200f, 100f);
		getTimeSinceRationingNode.name = "Get Time Since Rationing";
		getTimeSinceRationingNode.CreateInput("Character", "Character");
		getTimeSinceRationingNode.CreateInput("Consumable Object", "ConsumableRemedium");
		getTimeSinceRationingNode.CreateInput("Day Shift", "Bool");
		getTimeSinceRationingNode.CreateOutput("Last Rationing", "Int");
		return getTimeSinceRationingNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		GetTimeSinceRationingNode obj = (GetTimeSinceRationingNode)Create(rect.position + new Vector2(20f, 20f));
		obj._character = _character;
		obj._consumable = _consumable;
		obj._dayShift = _dayShift;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void OnNodeValidate()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		GetInputValue(Inputs[0], ref _character, canvas);
		GetInputValue(Inputs[1], ref _consumable, canvas);
		GetInputValue(Inputs[2], ref _dayShift, canvas);
		int num = SecondsRationingManager.Instance.TimeRationing.GetLastRationingTime(_consumable, _character);
		if (_dayShift)
		{
			int num2 = -1;
			CharacterList characterList = CharacterManager.Instance.GetCharacterList();
			for (int i = 0; i < characterList.GetCharacterCount(); i++)
			{
				if (characterList.CharactersInGame[i].Equals(_character))
				{
					num2 = i;
					break;
				}
			}
			List<Ration> rations = SecondsRationingManager.Instance.RationingData.Rations;
			num++;
			for (int j = 0; j < rations.Count; j++)
			{
				if (rations[j].CharacterIndex == num2 && rations[j].RationedItem.Equals(_consumable))
				{
					num = 0;
				}
			}
		}
		return CastValue<T>(num);
	}
}
