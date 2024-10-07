using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Remaster/Expedition Nodes/Get Expedition Time Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ConditionEvent)
})]
public class GetExpeditionTimeNode : ExpeditionNode
{
	public const string ID = "EE_GetExpeditionTimeNode";

	public const string NODE_NAME = "Get Expedition Time";

	public const string OUTPUT_TIME_EXPEDITION_LENGTH_NAME = "Expedition Length in Days";

	public const int OUTPUT_TIME_EXPEDITION_LENGTH_INDEX = 0;

	public const string OUTPUT_TIME_ELAPSED_NAME = "Elapsed Days";

	public const int OUTPUT_TIME_ELAPSED_INDEX = 1;

	public const string OUTPUT_TIME_LEFT_NAME = "Left Days";

	public const int OUTPUT_TIME_LEFT_INDEX = 2;

	public const string OUTPUT_TIME_MIN_NAME = "Min Days";

	public const int OUTPUT_TIME_MIN_INDEX = 3;

	public const string OUTPUT_TIME_MAX_NAME = "Max Days";

	public const int OUTPUT_TIME_MAX_INDEX = 4;

	public override string GetID => "EE_GetExpeditionTimeNode";

	public override Node Create(Vector2 pos)
	{
		GetExpeditionTimeNode getExpeditionTimeNode = ScriptableObject.CreateInstance<GetExpeditionTimeNode>();
		getExpeditionTimeNode.rect = new Rect(pos.x, pos.y, 180f, 90f);
		getExpeditionTimeNode.name = "Get Expedition Time";
		getExpeditionTimeNode.CreateOutput("Expedition Length in Days", "Int");
		getExpeditionTimeNode.CreateOutput("Elapsed Days", "Int");
		getExpeditionTimeNode.CreateOutput("Left Days", "Int");
		getExpeditionTimeNode.CreateOutput("Min Days", "Int");
		getExpeditionTimeNode.CreateOutput("Max Days", "Int");
		return getExpeditionTimeNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output > 4)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (!ExpeditionManager.Instance.IsExpeditionOngoing())
		{
			return CastValue<T>(0);
		}
		ExpeditionDestination currentDestination = ExpeditionManager.Instance.GetCurrentDestination();
		if (currentDestination == null && currentDestination.Event == null)
		{
			return CastValue<T>(0);
		}
		ExpeditionEvent @event = currentDestination.Event;
		return output switch
		{
			0 => CastValue<T>(@event.ExpeditionLength), 
			1 => CastValue<T>(ExpeditionManager.Instance.GetElapsedExpeditionTime()), 
			2 => CastValue<T>(@event.ExpeditionLength - ExpeditionManager.Instance.GetElapsedExpeditionTime()), 
			3 => CastValue<T>(@event.MinimumDaysAway), 
			4 => CastValue<T>(@event.MaximumDaysAway), 
			_ => throw new NotExistingOutputException(GetID, output), 
		};
	}
}
