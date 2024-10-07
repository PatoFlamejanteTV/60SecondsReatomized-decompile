using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class RPGConfig : Config
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
					value = 10f,
					min = 0f,
					max = 100f
				}
			},
			{
				"DistanceMin",
				new RangeParam
				{
					value = 2f,
					min = 0f,
					max = 100f
				}
			},
			{
				"DistanceMax",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 100f
				}
			},
			{
				"ZoomSpeed",
				new RangeParam
				{
					value = 0.5f,
					min = 0f,
					max = 10f
				}
			},
			{
				"EnableZoom",
				new BoolParam
				{
					value = true
				}
			},
			{
				"DefaultAngleX",
				new RangeParam
				{
					value = 45f,
					min = -180f,
					max = 180f
				}
			},
			{
				"EnableRotation",
				new BoolParam
				{
					value = true
				}
			},
			{
				"RotationSpeed",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"AngleY",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 85f
				}
			},
			{
				"AngleZoomMin",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 85f
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
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
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
				"Distance",
				new RangeParam
				{
					value = 10f,
					min = 0f,
					max = 100f
				}
			},
			{
				"DistanceMin",
				new RangeParam
				{
					value = 2f,
					min = 0f,
					max = 100f
				}
			},
			{
				"DistanceMax",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 100f
				}
			},
			{
				"ZoomSpeed",
				new RangeParam
				{
					value = 0.5f,
					min = 0f,
					max = 10f
				}
			},
			{
				"EnableZoom",
				new BoolParam
				{
					value = true
				}
			},
			{
				"DefaultAngleX",
				new RangeParam
				{
					value = 45f,
					min = -180f,
					max = 180f
				}
			},
			{
				"EnableRotation",
				new BoolParam
				{
					value = true
				}
			},
			{
				"RotationSpeed",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"AngleY",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 85f
				}
			},
			{
				"AngleZoomMin",
				new RangeParam
				{
					value = 50f,
					min = 0f,
					max = 85f
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
				"Spring",
				new RangeParam
				{
					value = 0f,
					min = 0f,
					max = 1f
				}
			},
			{
				"DeadZone",
				new Vector2Param
				{
					value = Vector2.zero
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
		Params = new Dictionary<string, Dictionary<string, Param>>
		{
			{ "Default", value },
			{ "Interior", value2 }
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
