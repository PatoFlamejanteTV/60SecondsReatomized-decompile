using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Remaster.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Text Nodes/Display History Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal)
})]
public class DisplayHistoryTextNode : MessageNode
{
	public const string ID = "DisplayHistoryTextNode";

	public override string GetID => "DisplayHistoryTextNode";

	public override Node Create(Vector2 pos)
	{
		DisplayHistoryTextNode displayHistoryTextNode = ScriptableObject.CreateInstance<DisplayHistoryTextNode>();
		displayHistoryTextNode.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayHistoryTextNode.name = "Display History Text";
		displayHistoryTextNode.CreateMutliInput("In", "Flow");
		displayHistoryTextNode.CreateOutput("Out", "Flow");
		return displayHistoryTextNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return (DisplayHistoryTextNode)Create(rect.position + new Vector2(20f, 20f));
	}

	public override void Execute(NodeCanvas canvas)
	{
		TextJournalContent content = new TextJournalContent(SimpleHistoryManager.Instance.RenderHistoryToString(), 0);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
