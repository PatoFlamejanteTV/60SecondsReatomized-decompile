using System;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Text Nodes/Term List", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal),
	typeof(ConditionEvent)
})]
public class TermListNode : ListNodeTemplate<LocalizedString>
{
	public override string GetID => "SE_TermsListNode";

	protected override string ConnectionType => "LocalizedString";

	public override Node Create(Vector2 pos)
	{
		TermListNode termListNode = ScriptableObject.CreateInstance<TermListNode>();
		termListNode.rect = new Rect(pos.x, pos.y, 100f, 85f);
		termListNode.name = "Term list";
		termListNode.CreateInput("Value 0", "LocalizedString");
		termListNode.CreateInput("Value 1", "LocalizedString");
		termListNode.CreateOutput("List", ListConnection.ID);
		return termListNode;
	}
}
