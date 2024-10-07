using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RuntimeMeshBatcher
{
	public static List<GameObject> rmbParents;

	public static Dictionary<GameObject, GameObject[]> originalObjetReferences;

	public static GameObject rmbContainerGameObject;

	public static GameObject CombineMeshes(LayerMask rmbLayer, bool processObjectsByTag = false, string rmbTag = null, bool processObjectsByLayer = false, bool destroyOriginalObjects = false, bool keepOriginalObjectReferences = false, bool combineByGrid = false, MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D, float gridSize = 0f)
	{
		List<GameObject> list = new List<GameObject>();
		if (processObjectsByTag && rmbTag != null)
		{
			list.AddRange(GameObject.FindGameObjectsWithTag(rmbTag));
		}
		if (processObjectsByLayer)
		{
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject gameObject in array)
			{
				if (IsInLayerMask(gameObject.layer, rmbLayer))
				{
					list.Add(gameObject);
				}
			}
		}
		return CombineMeshes(list.ToArray(), destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize);
	}

	private static bool IsInLayerMask(int layer, LayerMask layerMask)
	{
		int num = 1 << layer;
		if ((layerMask.value & num) > 0)
		{
			return true;
		}
		return false;
	}

	public static GameObject CombineMeshes(GameObject[] objectsToBatch, bool destroyOriginalObjects = false, bool keepOriginalObjectReferences = false, bool combineByGrid = false, MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D, float gridSize = 0f)
	{
		if (objectsToBatch == null || objectsToBatch.Length == 0)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: no objects found to be combined.");
			return null;
		}
		if (combineByGrid && gridSize <= 0f)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Grid Size must be superior to 0. Continuing batching without grid.");
			combineByGrid = false;
		}
		GameObject gameObject3;
		if (combineByGrid)
		{
			Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
			switch (gridType)
			{
			case MeshBatcherGridType.Grid2D:
			{
				GameObject[] array = objectsToBatch;
				foreach (GameObject gameObject2 in array)
				{
					Vector3 position2 = gameObject2.transform.position;
					int gridIndex4 = GetGridIndex(position2.x, gridSize);
					int gridIndex5 = GetGridIndex(position2.z, gridSize);
					string key2 = gridIndex4 + "_" + gridIndex5;
					if (!dictionary.ContainsKey(key2))
					{
						dictionary[key2] = new List<GameObject>();
					}
					dictionary[key2].Add(gameObject2);
				}
				break;
			}
			case MeshBatcherGridType.Grid3D:
			{
				GameObject[] array = objectsToBatch;
				foreach (GameObject gameObject in array)
				{
					Vector3 position = gameObject.transform.position;
					int gridIndex = GetGridIndex(position.x, gridSize);
					int gridIndex2 = GetGridIndex(position.y, gridSize);
					int gridIndex3 = GetGridIndex(position.z, gridSize);
					string key = gridIndex + "_" + gridIndex2 + "_" + gridIndex3;
					if (!dictionary.ContainsKey(key))
					{
						dictionary[key] = new List<GameObject>();
					}
					dictionary[key].Add(gameObject);
				}
				break;
			}
			}
			gameObject3 = CreateParent();
			foreach (List<GameObject> value in dictionary.Values)
			{
				SetParent(CreateStaticMesh(value.ToArray(), destroyOriginalObjects, combineByGrid: true), gameObject3);
			}
		}
		else
		{
			gameObject3 = CreateStaticMesh(objectsToBatch, destroyOriginalObjects);
		}
		if (keepOriginalObjectReferences)
		{
			if (originalObjetReferences == null)
			{
				originalObjetReferences = new Dictionary<GameObject, GameObject[]>();
			}
			originalObjetReferences[gameObject3] = objectsToBatch;
		}
		return gameObject3;
	}

	private static int GetGridIndex(float position, float gridSize)
	{
		return (int)Mathf.Floor(position / gridSize);
	}

	private static GameObject CreateParent()
	{
		if (rmbParents == null)
		{
			rmbParents = new List<GameObject>();
		}
		GameObject gameObject = new GameObject("RuntimeMeshBatcherParent_" + rmbParents.Count);
		rmbParents.Add(gameObject);
		if (rmbContainerGameObject == null)
		{
			rmbContainerGameObject = new GameObject("RuntimeMeshBatcherContainer");
		}
		SetParent(gameObject, rmbContainerGameObject);
		return gameObject;
	}

	private static GameObject CreateStaticMesh(GameObject[] gameObjects, bool destroyOriginalObjects, bool combineByGrid = false)
	{
		GameObject gameObject2 = (combineByGrid ? new GameObject("SubParent") : CreateParent());
		Transform transform = gameObject2.transform;
		Dictionary<Transform, Transform> originalTransforms = new Dictionary<Transform, Transform>();
		foreach (GameObject gameObject3 in gameObjects)
		{
			if (!(gameObject3 == null) && gameObject3.GetComponentsInChildren<Renderer>().Length != 0 && !gameObject3.GetComponentsInChildren<Renderer>()[0].isPartOfStaticBatch)
			{
				Transform transform2 = gameObject3.transform;
				if (!destroyOriginalObjects)
				{
					originalTransforms[transform2] = transform2.parent;
				}
				SetParent(transform2, transform);
			}
		}
		Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
		Dictionary<Material, List<List<CombineInstance>>> dictionary = new Dictionary<Material, List<List<CombineInstance>>>();
		MeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Material[] sharedMaterials = array[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material != null && !dictionary.ContainsKey(material))
				{
					dictionary.Add(material, new List<List<CombineInstance>>());
				}
			}
		}
		Dictionary<Material, int> dictionary2 = new Dictionary<Material, int>();
		MeshFilter[] componentsInChildren2 = gameObject2.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren2)
		{
			if (meshFilter.sharedMesh == null)
			{
				continue;
			}
			if (meshFilter.sharedMesh.subMeshCount > 1)
			{
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				for (int k = 0; k < component.sharedMaterials.Count(); k++)
				{
					CombineInstance combineInstance = default(CombineInstance);
					combineInstance.mesh = meshFilter.sharedMesh;
					combineInstance.subMeshIndex = k;
					combineInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
					CombineInstance item = combineInstance;
					Material key = component.sharedMaterials[k];
					int vertexCount = item.mesh.vertexCount;
					if (!dictionary2.ContainsKey(key) || dictionary2[key] + vertexCount > 65536)
					{
						dictionary2[key] = 0;
						dictionary[key].Add(new List<CombineInstance>());
					}
					int count = dictionary[key].Count;
					List<CombineInstance> list = dictionary[key][count - 1];
					dictionary2[key] += vertexCount;
					list.Add(item);
				}
			}
			else
			{
				CombineInstance combineInstance = default(CombineInstance);
				combineInstance.mesh = meshFilter.sharedMesh;
				combineInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
				CombineInstance item2 = combineInstance;
				Material sharedMaterial = meshFilter.GetComponent<Renderer>().sharedMaterial;
				int vertexCount2 = item2.mesh.vertexCount;
				if (!dictionary2.ContainsKey(sharedMaterial) || dictionary2[sharedMaterial] + vertexCount2 > 65536)
				{
					dictionary2[sharedMaterial] = 0;
					dictionary[sharedMaterial].Add(new List<CombineInstance>());
				}
				int count2 = dictionary[sharedMaterial].Count;
				List<CombineInstance> list2 = dictionary[sharedMaterial][count2 - 1];
				dictionary2[sharedMaterial] += vertexCount2;
				list2.Add(item2);
			}
			meshFilter.GetComponent<Renderer>().enabled = false;
		}
		foreach (Material key2 in dictionary.Keys)
		{
			for (int l = 0; l < dictionary[key2].Count; l++)
			{
				List<CombineInstance> list3 = dictionary[key2][l];
				GameObject gameObject4 = new GameObject("CombinedMesh_" + key2.name + "_" + l);
				Transform transform3 = gameObject4.transform;
				SetParent(transform3, gameObject2.transform);
				transform3.localPosition = Vector3.zero;
				transform3.localRotation = Quaternion.identity;
				transform3.localScale = Vector3.one;
				gameObject4.AddComponent<MeshFilter>().mesh.CombineMeshes(list3.ToArray(), mergeSubMeshes: true, useMatrices: true);
				gameObject4.AddComponent<MeshRenderer>().material = key2;
			}
		}
		if (destroyOriginalObjects)
		{
			for (int m = 0; m < gameObjects.Length; m++)
			{
				Object.Destroy(gameObjects[m]);
			}
		}
		else
		{
			array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			foreach (Transform item3 in from gameObject in gameObjects
				select gameObject.transform into goTransform
				where originalTransforms.ContainsKey(goTransform)
				select goTransform)
			{
				if (item3 != null && originalTransforms[item3] != null)
				{
					SetParent(item3, originalTransforms[item3]);
				}
			}
		}
		return gameObject2;
	}

	public static void UncombineMeshes(GameObject rmbParent)
	{
		if (rmbParent == null)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes with null parent GameObject.");
			return;
		}
		if (originalObjetReferences == null)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes without having kept object references.");
			return;
		}
		if (!originalObjetReferences.ContainsKey(rmbParent))
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes with undefined parent GameObject.");
			return;
		}
		GameObject[] array = originalObjetReferences[rmbParent];
		Object.Destroy(rmbParent);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject != null)
			{
				MeshRenderer[] components = gameObject.GetComponents<MeshRenderer>();
				for (int j = 0; j < components.Length; j++)
				{
					components[j].enabled = true;
				}
				components = gameObject.GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < components.Length; j++)
				{
					components[j].enabled = true;
				}
			}
		}
	}

	private static void SetParent(GameObject gameObject, GameObject parent)
	{
		SetParent(gameObject.transform, parent.transform);
	}

	private static void SetParent(Transform gameObjectTransform, Transform parentTransform)
	{
		gameObjectTransform.SetParent(parentTransform, worldPositionStays: true);
	}
}
