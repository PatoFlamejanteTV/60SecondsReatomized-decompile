using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Remaster.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Text Nodes/Display History Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal)
})]
public class DisplayHistoryTextNodeV2 : MessageNode
{
	public const string ID = "DisplayHistoryTextNodeV2";

	private const string NODE_NAME = "Display History Text";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_PRIORITY_NAME = "Display Priority";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_PRIORITY_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	[SerializeField]
	private int _displayPriority;

	public override string GetID => "DisplayHistoryTextNodeV2";

	public override Node Create(Vector2 pos)
	{
		DisplayHistoryTextNodeV2 displayHistoryTextNodeV = ScriptableObject.CreateInstance<DisplayHistoryTextNodeV2>();
		displayHistoryTextNodeV.rect = new Rect(pos.x, pos.y, 300f, 105f);
		displayHistoryTextNodeV.name = "Display History Text";
		displayHistoryTextNodeV.CreateMutliInput("In", "Flow");
		displayHistoryTextNodeV.CreateInput("Display Priority", "Int");
		displayHistoryTextNodeV.CreateOutput("Out", "Flow");
		return displayHistoryTextNodeV;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayHistoryTextNodeV2 obj = (DisplayHistoryTextNodeV2)Create(rect.position + new Vector2(20f, 20f));
		obj._displayPriority = _displayPriority;
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
		GetInputValue(Inputs[1], ref _displayPriority, canvas);
		TextJournalContent content = new TextJournalContent(SimpleHistoryManager.Instance.RenderHistoryToString(), _displayPriority);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
