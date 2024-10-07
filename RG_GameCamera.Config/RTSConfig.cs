using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config;

public class RTSConfig : Config
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
				"EnableRotation",
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
				"RotationSpeed",
				new RangeParam
				{
					value = 8f,
					min = 0f,
					max = 10f
				}
			},
			{
				"GroundOffset",
				new RangeParam
				{
					value = 0f,
					min = -100f,
					max = 100f
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
				"FollowTargetY",
				new BoolParam
				{
					value = true
				}
			},
			{
				"DraggingMove",
				new BoolParam
				{
					value = true
				}
			},
			{
				"ScreenBorderMove",
				new BoolParam
				{
					value = true
				}
			},
			{
				"ScreenBorderOffset",
				new RangeParam
				{
					value = 10f,
					min = 1f,
					max = 500f
				}
			},
			{
				"ScreenBorderSpeed",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"KeyMove",
				new BoolParam
				{
					value = true
				}
			},
			{
				"MoveSpeed",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 10f
				}
			},
			{
				"MapCenter",
				new Vector2Param
				{
					value = Vector2.zero
				}
			},
			{
				"MapSize",
				new Vector2Param
				{
					value = new Vector2(100f, 100f)
				}
			},
			{
				"SoftBorder",
				new RangeParam
				{
					value = 5f,
					min = 0f,
					max = 20f
				}
			},
			{
				"DisableHorizontal",
				new BoolParam
				{
					value = false
				}
			},
			{
				"DisableVertical",
				new BoolParam
				{
					value = false
				}
			},
			{
				"DragMomentum",
				new RangeParam
				{
					value = 1f,
					min = 0f,
					max = 3f
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
