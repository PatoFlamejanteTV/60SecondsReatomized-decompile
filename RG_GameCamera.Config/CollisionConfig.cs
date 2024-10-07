using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class CollisionConfig : Config
{
	public override void LoadDefault()
	{
		Dictionary<string, Param> dictionary = new Dictionary<string, Param>();
		dictionary.Add("MinDistance", new RangeParam
		{
			value = 0.5f,
			min = 0f,
			max = 10f
		});
		dictionary.Add("ReturnSpeed", new RangeParam
		{
			value = 0.4f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("ClipSpeed", new RangeParam
		{
			value = 0.05f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("IgnoreCollisionTag", new StringParam
		{
			value = "Player"
		});
		dictionary.Add("TransparentCollisionTag", new StringParam
		{
			value = "CameraTransparent"
		});
		dictionary.Add("TargetSphereRadius", new RangeParam
		{
			value = 0.5f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("RaycastTolerance", new RangeParam
		{
			value = 0.5f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("TargetClipSpeed", new RangeParam
		{
			value = 0.1f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("ReturnTargetSpeed", new RangeParam
		{
			value = 0.1f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("SphereCastRadius", new RangeParam
		{
			value = 0.1f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("SphereCastTolerance", new RangeParam
		{
			value = 0.5f,
			min = 0f,
			max = 1f
		});
		dictionary.Add("CollisionAlgorithm", new SelectionParam
		{
			index = 0,
			value = new string[3] { "Simple", "Spherical", "Volumetric" }
		});
		dictionary.Add("ConeRadius", new Vector2Param
		{
			value = new Vector2(0.5f, 1f)
		});
		dictionary.Add("ConeSegments", new RangeParam
		{
			value = 3f,
			min = 3f,
			max = 10f
		});
		dictionary.Add("HeadOffset", new RangeParam
		{
			value = 1.6f,
			min = 0f,
			max = 10f
		});
		Dictionary<string, Param> value = dictionary;
		Params = new Dictionary<string, Dictionary<string, Param>> { { "Default", value } };
		Transitions = new Dictionary<string, float>();
		foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
		{
			Transitions.Add(param.Key, 0.25f);
		}
		Deserialize(base.DefaultConfigPath);
		base.LoadDefault();
	}

	protected override void Awake()
	{
		base.Awake();
		LoadDefault();
	}
}
