using System;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(false, "Text Nodes/Display Text Icon Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class DisplayTextIconNode : ResourceNode
{
	public const string ID = "EE_DisplayTextIconNode";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ICON_TERM_NAME = "Icon Term";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string INPUT_PRIORITY_NAME = "Display Priority";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ICON_TERM_INDEX = 1;

	private const int INPUT_AMOUNT_INDEX = 2;

	private const int INPUT_PRIORITY_INDEX = 3;

	private const int OUTPUT_OUT_INDEX = 0;

	[SerializeField]
	private LocalizedString _term;

	[SerializeField]
	private int _amount;

	[SerializeField]
	private int _priority;

	[SerializeField]
	private EventContentData.ETextIconContentType _type;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_DisplayTextIconNode";

	public override Node Create(Vector2 pos)
	{
		DisplayTextIconNode displayTextIconNode = ScriptableObject.CreateInstance<DisplayTextIconNode>();
		displayTextIconNode.rect = new Rect(pos.x, pos.y, 250f, 130f);
		displayTextIconNode.name = "Display Text Icon";
		displayTextIconNode.CreateMutliInput("In", "Flow");
		displayTextIconNode.CreateInput("Icon Term", "LocalizedString");
		displayTextIconNode.CreateInput("Amount", "Int");
		displayTextIconNode.CreateInput("Display Priority", "Int");
		displayTextIconNode.CreateOutput("Out", "Flow");
		return displayTextIconNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayTextIconNode obj = (DisplayTextIconNode)Create(rect.position + new Vector2(20f, 20f));
		obj._term = _term;
		obj._amount = _amount;
		obj._type = _type;
		obj._priority = _priority;
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
		GetInputValue(Inputs[1], ref _term, canvas);
		GetInputValue(Inputs[2], ref _amount, canvas);
		GetInputValue(Inputs[3], ref _priority, canvas);
		TextIconJournalContent content = new TextIconJournalContent(_term, _amount, _type, _priority);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
