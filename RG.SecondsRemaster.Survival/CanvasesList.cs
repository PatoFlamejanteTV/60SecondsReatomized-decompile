using System.Collections.Generic;
using NodeEditorFramework;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/New Canvases List", fileName = "New Canvases List")]
public class CanvasesList : ScriptableObject
{
	[SerializeField]
	private List<NodeCanvas> _canvases;

	public List<NodeCanvas> Canvases => _canvases;

	public bool Contains(NodeCanvas canvas)
	{
		return _canvases.Contains(canvas);
	}
}
