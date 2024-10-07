using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Get Resource Result Node", new Type[] { typeof(SurvivalEvent) })]
public class GetResourceResultNode : PlayerDecisionNode
{
	private const string ID = "EE_GetResourceResultNode";

	private const string INPUT_RESULT_NAME = "Result";

	private const string INPUT_OUTPUT_RES_1_NAME = "Resource 1";

	private const string INPUT_OUTPUT_RES_2_NAME = "Resource 2";

	private const string INPUT_OUTPUT_RES_3_NAME = "Resource 3";

	private const string OUTPUT_NO_CHOICE_NAME = "No Choice";

	private const string OUTPUT_RES_NAME = "Resource";

	private const string NODE_NAME = "Get Resource Result";

	private const int INPUT_RESULT_INDEX = 1;

	private const int OUTPUT_RES_1_INDEX = 0;

	private const int OUTPUT_RES_2_INDEX = 1;

	private const int OUTPUT_RES_3_INDEX = 2;

	private const int OUTPUT_NO_CHOICE_INDEX = 3;

	private const int OUTPUT_RES_INDEX = 4;

	private const int RES_1_CARD_ID = 3;

	private const int RES_2_CARD_ID = 2;

	private const int RES_3_CARD_ID = 1;

	private const int NO_CHOICE_CARD_ID = 0;

	[SerializeField]
	private PlayerDecision _currentDecision;

	public override string GetID => "EE_GetResourceResultNode";

	public override Node Create(Vector2 pos)
	{
		GetResourceResultNode getResourceResultNode = ScriptableObject.CreateInstance<GetResourceResultNode>();
		getResourceResultNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		getResourceResultNode.name = "Get Resource Result";
		getResourceResultNode.CreateMutliInput("In", "Flow");
		getResourceResultNode.CreateInput("Result", "PlayerDecision");
		getResourceResultNode.CreateOutput("Resource 1", "Flow");
		getResourceResultNode.CreateOutput("Resource 2", "Flow");
		getResourceResultNode.CreateOutput("Resource 3", "Flow");
		getResourceResultNode.CreateOutput("No Choice", "Flow");
		getResourceResultNode.CreateOutput("Resource", "Resources");
		return getResourceResultNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void OnNodeValidate()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _currentDecision, canvas);
		if (!(_currentDecision is PlayerResourceDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerResourceDecision), _currentDecision.GetType());
		}
		PlayerResourceDecision playerResourceDecision = (PlayerResourceDecision)_currentDecision;
		GameResource result = playerResourceDecision.Result;
		if (base.ParentEvent is SurvivalEvent)
		{
			((SurvivalEvent)base.ParentEvent).WasEventSuccessful = playerResourceDecision.ChoosenNumber != 0;
		}
		if (playerResourceDecision.ChoosenNumber == 0)
		{
			if (!Outputs[3].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 3);
			}
			Outputs[3].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (playerResourceDecision.ChoosenNumber == 3)
		{
			if (playerResourceDecision.UseResource)
			{
				UseResource(result);
			}
			if (!Outputs[0].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 0);
			}
			Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (playerResourceDecision.ChoosenNumber == 2)
		{
			if (playerResourceDecision.UseResource)
			{
				UseResource(result);
			}
			if (!Outputs[1].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 1);
			}
			Outputs[1].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (playerResourceDecision.ChoosenNumber == 1)
		{
			if (playerResourceDecision.UseResource)
			{
				UseResource(result);
			}
			if (!Outputs[2].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 2);
			}
			Outputs[2].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		throw new WrongDecisionConnectionExcpetion(GetID);
	}

	private void UseResource(GameResource gameResource)
	{
		PlayerResources playerResources = Singleton<ItemManager>.Instance.GetPlayerResources();
		playerResources.Unlock(gameResource);
		int num = playerResources.RemoveResourceAndGetRemovedAmount(gameResource.Resource, gameResource.Amount);
		ParsecsEventManager.DisplayTextIconContent(base.ParentCanvas, EventContentData.ETextIconContentType.SUBTRACTION, num, gameResource.Resource);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 4)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (!(_currentDecision is PlayerResourceDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerResourceDecision), _currentDecision.GetType());
		}
		GameResource result = ((PlayerResourceDecision)_currentDecision).Result;
		return CastValue<T>(result);
	}
}
