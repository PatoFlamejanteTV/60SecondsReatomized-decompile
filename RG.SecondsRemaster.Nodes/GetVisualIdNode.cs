using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Utility Nodes/Get VisualId Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(EndGameCanvas)
})]
public class GetVisualIdNode : MessageNode
{
	public const string ID = "EE_GetVisualIdNode";

	[SerializeField]
	private VisualId _visualId;

	public override string GetID => "EE_GetVisualIdNode";

	public override Node Create(Vector2 pos)
	{
		GetVisualIdNode getVisualIdNode = ScriptableObject.CreateInstance<GetVisualIdNode>();
		getVisualIdNode.rect = new Rect(pos.x, pos.y, 300f, 70f);
		getVisualIdNode.name = "Get VisualId";
		getVisualIdNode.CreateOutput("Out", "VisualId");
		return getVisualIdNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		GetVisualIdNode obj = (GetVisualIdNode)Create(rect.position + new Vector2(20f, 20f));
		obj._visualId = _visualId;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	protected override void OnNodeValidate()
	{
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		return CastValue<T>(_visualId);
	}
}
