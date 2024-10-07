using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Text Nodes/Display Pure Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent)
})]
public class DisplayPureTextNode : MessageNode
{
	public const string ID = "EE_DisplayPureTextNode";

	[SerializeField]
	private string _text;

	public override string GetID => "EE_DisplayPureTextNode";

	public override Node Create(Vector2 pos)
	{
		DisplayPureTextNode displayPureTextNode = ScriptableObject.CreateInstance<DisplayPureTextNode>();
		displayPureTextNode.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayPureTextNode.name = "Display Pure Text";
		displayPureTextNode.CreateMutliInput("In", "Flow");
		displayPureTextNode.CreateInput("Term", "String");
		displayPureTextNode.CreateOutput("Out", "Flow");
		return displayPureTextNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayPureTextNode obj = (DisplayPureTextNode)Create(rect.position + new Vector2(20f, 20f));
		obj._text = _text;
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

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _text, canvas);
		TextJournalContent content = new TextJournalContent(_text, 0);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
