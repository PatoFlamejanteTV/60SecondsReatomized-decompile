using DunGen.Graph;
using UnityEngine;

namespace DunGen;

[AddComponentMenu("DunGen/Tile")]
public class Tile : MonoBehaviour
{
	public bool AllowRotation = true;

	[SerializeField]
	private TilePlacementData placement;

	[SerializeField]
	private DungeonArchetype archetype;

	[SerializeField]
	private TileSet tileSet;

	[SerializeField]
	private FlowNodeReference node;

	[SerializeField]
	private FlowLineReference line;

	public TilePlacementData Placement
	{
		get
		{
			return placement;
		}
		internal set
		{
			placement = value;
		}
	}

	public DungeonArchetype Archetype
	{
		get
		{
			return archetype;
		}
		internal set
		{
			archetype = value;
		}
	}

	public TileSet TileSet
	{
		get
		{
			return tileSet;
		}
		internal set
		{
			tileSet = value;
		}
	}

	public GraphNode Node
	{
		get
		{
			if (node != null)
			{
				return node.Node;
			}
			return null;
		}
		internal set
		{
			if (value == null)
			{
				node = null;
			}
			else
			{
				node = new FlowNodeReference(value.Graph, value);
			}
		}
	}

	public GraphLine Line
	{
		get
		{
			if (line != null)
			{
				return line.Line;
			}
			return null;
		}
		internal set
		{
			if (value == null)
			{
				line = null;
			}
			else
			{
				line = new FlowLineReference(value.Graph, value);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (placement != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(placement.Bounds.center, placement.Bounds.size);
		}
	}
}
