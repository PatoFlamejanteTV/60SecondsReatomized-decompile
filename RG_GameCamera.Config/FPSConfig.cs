using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class FPSConfig : Config
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
				"RotationSpeedX",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationSpeedY",
				new RangeParam
				{
					value = 5f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
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
				"Orthographic",
				new BoolParam
				{
					value = false
				}
			},
			{
				"HideTarget",
				new BoolParam
				{
					value = true
				}
			}
		};
		Dictionary<string, Param> value2 = new Dictionary<string, Param>
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
				"RotationSpeedX",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationSpeedY",
				new RangeParam
				{
					value = 5f,
					min = 0f,
					max = 10f
				}
			},
			{
				"RotationYMax",
				new RangeParam
				{
					value = 80f,
					min = 0f,
					max = 80f
				}
			},
			{
				"RotationYMin",
				new RangeParam
				{
					value = -80f,
					min = -80f,
					max = 0f
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
				"Orthographic",
				new BoolParam
				{
					value = false
				}
			},
			{
				"HideTarget",
				new BoolParam
				{
					value = true
				}
			}
		};
		Params = new Dictionary<string, Dictionary<string, Param>>
		{
			{ "Default", value },
			{ "Crouch", value2 }
		};
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
