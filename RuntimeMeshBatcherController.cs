using UnityEngine;

public class RuntimeMeshBatcherController : MonoBehaviour
{
	public bool processObjectsByLayer;

	public bool processObjectsByTag;

	public LayerMask rmbLayer;

	public string rmbTag;

	public bool destroyOriginalObjects;

	public bool keepOriginalObjectReferences;

	public bool combineByGrid;

	public MeshBatcherGridType gridType;

	public float gridSize;

	public bool autoRun;

	public static RuntimeMeshBatcherController instance { get; set; }

	protected void Awake()
	{
		instance = this;
		RuntimeMeshBatcher.rmbContainerGameObject = base.gameObject;
	}

	protected void Start()
	{
		if (autoRun)
		{
			CombineMeshes();
		}
	}

	public GameObject CombineMeshes()
	{
		return RuntimeMeshBatcher.CombineMeshes(rmbLayer, processObjectsByTag, rmbTag, processObjectsByLayer, destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize);
	}

	public void UncombineMeshes(GameObject rmbParent)
	{
		RuntimeMeshBatcher.UncombineMeshes(rmbParent);
	}

	public GameObject CombineMeshes(GameObject[] objectsToBatch)
	{
		return RuntimeMeshBatcher.CombineMeshes(objectsToBatch, destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize);
	}
}
