using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class DeadConfig : Config
{
	public override void LoadDefault()
	{
		Dictionary<string, Param> value = new Dictionary<string, Param>
		{
			{
				"FOV",
				new RangeParam
				{
					value = 60f,
					min = 20f,
					max = 100f
				}
			},
			{
				"Distance",
				new RangeParam
				{
					value = 2f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationSpeed",
				new RangeParam
				{
					value = 0.5f,
					min = -10f,
					max = 10f
				}
			},
			{
				"Angle",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 80f
				}
			},
			{
				"TargetOffset",
				new Vector3Param
				{
					value = Vector3.zero
				}
			},
			{
				"Collision",
				new BoolParam
				{
					value = true
				}
			},
			{
				"Orthographic",
				new BoolParam
				{
					value = false
				}
			}
		};
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
