using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

public abstract class ListNodeTemplate<InputType> : ParsecsNode
{
	[SerializeField]
	private int _counter = 2;

	private const int OUTPUT_INDEX = 0;

	private const int MINIMAL_INPUTS = 1;

	protected abstract string ConnectionType { get; }

	public abstract override Node Create(Vector2 pos);

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void NodeGUI()
	{
	}

	private void AddNewElement()
	{
		CreateInput("Value " + _counter, ConnectionType);
		_counter++;
	}

	private void RemoveElement()
	{
		Inputs[Inputs.Count - 1].Delete();
		_counter--;
	}

	protected override void OnNodeValidate()
	{
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		List<InputType> list = new List<InputType>();
		for (int i = 0; i < Inputs.Count; i++)
		{
			list.Add(GetInputValue<InputType>(Inputs[i], canvas));
		}
		return CastValue<T>(list);
	}
}
